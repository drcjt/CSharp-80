using ILCompiler.Compiler.CodeGenerators;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.Peephole;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Dnlib;
using System.Diagnostics;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler
{
    public class CodeGenerator : ICodeGenerator, IGenericStackEntryVisitor
    {
        private readonly INameMangler _nameMangler;
        private readonly ICodeGeneratorFactory _codeGeneratorFactory;
        private readonly IConfiguration _configuration;
        private readonly CorLibModuleProvider _corLibModuleProvider;
        private readonly NodeFactory _nodeFactory;
        private readonly Optimizer _optimizer;

        private CodeGeneratorContext _context = null!;

        public CodeGenerator(INameMangler nameMangler, ICodeGeneratorFactory codeGeneratorFactory, IConfiguration configuration, 
            CorLibModuleProvider corLibModuleProvider, NodeFactory nodeFactory, Optimizer optimizer)
        {
            _nameMangler = nameMangler;
            _codeGeneratorFactory = codeGeneratorFactory;
            _configuration = configuration;
            _corLibModuleProvider = corLibModuleProvider;
            _nodeFactory = nodeFactory;
            _optimizer = optimizer;
        }

        public IList<Instruction> Generate(IList<BasicBlock> blocks, LocalVariableTable locals, Z80MethodCodeNode methodCodeNode)
        {
            _context = new CodeGeneratorContext(locals, methodCodeNode, _configuration, _nameMangler, _nodeFactory, _corLibModuleProvider);

            AssignFrameOffsets();

            var methodInstructions = new List<Instruction>();

            string methodName;
            if (methodCodeNode.Method.HasCustomAttribute("System.Runtime", "RuntimeExportAttribute"))
            {
                methodName = ((DnlibMethod)(methodCodeNode.Method)).GetRuntimeExportName();
            }
            else
            {
                methodName = _nameMangler.GetMangledMethodName(_context.Method);
            }

            if (Compilation.AnyExceptionHandlers)
            {
                GenerateMethodUnwindInfo();
            }

            _context.InstructionsBuilder.Label(methodName);

            if (methodCodeNode.Method.IsStaticConstructor)
            {
                // When a static constructor finishes running it replaces the first instruction in
                // the method with a Ret instruction so it only ever executes once even if called multiple times
                _context.InstructionsBuilder.Ld(HL, methodName);
                _context.InstructionsBuilder.Ld(__[HL], 0xC9);
            }

            GenerateProlog(_context.InstructionsBuilder);
            methodInstructions.AddRange(_context.InstructionsBuilder.Instructions);

            foreach (var block in blocks)
            {
                _context.InstructionsBuilder.Reset();

                _context.InstructionsBuilder.Label(block.Label);

                var visitorAdapter = new GenericStackEntryAdapter(this);

                var currentNode = block.FirstNode;
                while (currentNode != null)
                {
                    // Only need to generate code for nodes that are not contained
                    // Contained nodes are part of their parents for code generation purposes
                    if (!currentNode.Contained)
                    {
                        currentNode.Accept(visitorAdapter);
                    }
                    currentNode = currentNode.Next;
                }

                if (block.JumpKind == JumpKind.Always)
                {
                    var nonHandlerSuccessors = block.Successors.Where(s => !s.HandlerStart).ToList();
                    if (nonHandlerSuccessors.Count == 1)
                    {
                        _context.InstructionsBuilder.Jp(nonHandlerSuccessors[0].Label);
                    }
                }

                methodInstructions.AddRange(_context.InstructionsBuilder.Instructions);
            }
            // Emit end of method label
            _context.InstructionsBuilder.Reset();
            _context.InstructionsBuilder.Label($"{methodName}_END");
            methodInstructions.AddRange(_context.InstructionsBuilder.Instructions);

            _optimizer.Optimize(methodInstructions);

            var totalBytes = methodInstructions.Sum<Instruction>(x => x.Bytes);

            methodInstructions.Add(Instruction.CreateComment($"Total bytes of code {totalBytes} for method {methodCodeNode.Method.FullName}"));

            return methodInstructions;
        }

        private void GenerateMethodUnwindInfo()
        {
            // Think of this as the unwind information
            // Could expand from single byte for number of parameters
            // For example to handle frameless methods.

            // Emit byte before method as number of bytes parameters take up on stack - basically unwind info for exception handling
            var totalParametersSize = 0;
            foreach (var local in _context.LocalVariableTable)
            {
                if (local.IsParameter)
                {
                    totalParametersSize += local.ExactSize;
                }
            }
            Debug.Assert(totalParametersSize <= Byte.MaxValue);
            _context.InstructionsBuilder.Db((byte)totalParametersSize, "Total Parameter Size");
        }

        private void AssignFrameOffsets()
        {
            // First process the arguments
            var totalArgumentsSize = AssignFrameOffsetsToArguments();

            // Calculate the frame offsets for local variables
            AssignFrameOffsetsToLocals();

            // Patch the offsets
            FixFrameOffsets(totalArgumentsSize);
        }

        private void FixFrameOffsets(int totalArgumentsSize)
        {
            // Now fixup offset for parameters given that we now know the overall size of the parameters
            // and also take into account the return address and frame pointer (IX)
            foreach (var variable in _context.LocalVariableTable)
            {
                if (variable.IsParameter)
                {
                    // return address occupies 2 bytes
                    // frame pointer occupies 2 bytes
                    variable.StackOffset -= (totalArgumentsSize + 2 + 2);
                }
            }
        }

        private int AssignFrameOffsetsToArguments()
        {
            var totalArgumentsSize = 0;
            var offset = 0;

            foreach (var localVariable in _context.LocalVariableTable)
            {
                if (localVariable.IsParameter)
                {
                    var argumentSize = Math.Max(2, localVariable.ExactSize);  // Minimum size for parameters on stack is 2 bytes
                    localVariable.ExactSize = argumentSize;
                    localVariable.StackOffset = offset + localVariable.ExactSize;

                    offset += argumentSize;
                    totalArgumentsSize += argumentSize;
                }
            }

            return totalArgumentsSize;
        }

        private void AssignFrameOffsetsToLocals()
        {
            var offset = 0;
            foreach (var localVariable in _context.LocalVariableTable)
            {
                if (!localVariable.IsParameter)
                {
                    localVariable.StackOffset = offset + localVariable.ExactSize;
                    offset += localVariable.ExactSize;
                }
            }
        }

        public void Visit<T>(T entry) where T : StackEntry
        {
            // Contained nodes are part of their parents from a code generation perspective
            if (!entry.Contained)
            {
                _codeGeneratorFactory.GetCodeGenerator<T>().GenerateCode(entry, _context);
            }
        }

        private void GenerateProlog(InstructionsBuilder instructionsBuilder)
        {
            // Stack frame looks like this:
            //
            //     |                       |
            //     |-----------------------|
            //     |       incoming        |
            //     |       arguments       |
            //     |-----------------------|
            //     |    return address     |
            //     +=======================+
            //     |     IX (optional)     |    Not present if no locals or params
            //     |-----------------------|   <-- IX will point to here when method code executes
            //     |    Local variables    |
            //     |-----------------------|
            //     |   Arguments for the   |
            //     ~     next method       ~
            //     |                       |
            //     |      |                |
            //     |      | Stack grows    |
            //            | downward
            //            V

            var localsSize = 0;
            var tempCount = 0;
            foreach (var localVariable in _context.LocalVariableTable)
            {
                if (localVariable.IsTemp)
                {
                    tempCount++;
                }
                if (!localVariable.IsParameter)
                {
                    localsSize += localVariable.ExactSize;
                }
            }

            instructionsBuilder.Push(IX);
            instructionsBuilder.Ld(IX, 0);
            instructionsBuilder.Add(IX, SP);

            if (_context.LocalsCount + tempCount > 0)
            {
                // Reserve space on stack for locals
                CodeGeneratorHelper.AddSPFromHL(instructionsBuilder, (short)-localsSize);

                ZeroInitFrame(instructionsBuilder);
            }
        }

        private void ZeroInitFrame(InstructionsBuilder instructionsBuilder)
        {
            if (_context.Method.MethodIL!.IsInitLocals)
            {
                foreach (var variable in _context.LocalVariableTable)
                {
                    if (variable.MustInit)
                    {
                        // Emit code to init the local here
                        var offset = variable.StackOffset;
                        var exactSize = variable.ExactSize;

                        instructionsBuilder.Push(IX);
                        instructionsBuilder.Pop(HL);

                        CodeGeneratorHelper.AddHLFromDE(instructionsBuilder, (short)-offset);

                        for (var count = 0; count < exactSize; count++)
                        {
                            instructionsBuilder.Ld(__[HL], 0);
                            instructionsBuilder.Inc(HL);
                        }
                    }
                }
            }
        }
    }
}
