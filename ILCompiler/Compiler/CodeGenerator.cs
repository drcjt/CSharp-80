using ILCompiler.Compiler.CodeGenerators;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using Microsoft.Extensions.Logging;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler
{
    public class CodeGenerator : ICodeGenerator, IGenericStackEntryVisitor
    {
        private readonly INameMangler _nameMangler;
        private readonly ILogger<CodeGenerator> _logger;
        private readonly ICodeGeneratorFactory _codeGeneratorFactory;
        private readonly IConfiguration _configuration;
        private readonly CorLibModuleProvider _corLibModuleProvider;

        private readonly Dictionary<string, string> _labelsToStringData = new();

        private CodeGeneratorContext _context = null!;

        public CodeGenerator(INameMangler nameMangler, ILogger<CodeGenerator> logger, ICodeGeneratorFactory codeGeneratorFactory, IConfiguration configuration, CorLibModuleProvider corLibModuleProvider)
        {
            _nameMangler = nameMangler;
            _logger = logger;
            _codeGeneratorFactory = codeGeneratorFactory;
            _configuration = configuration;
            _corLibModuleProvider = corLibModuleProvider;
        }

        public IList<Instruction> Generate(IList<BasicBlock> blocks, IList<LocalVariableDescriptor> localVariableTable, Z80MethodCodeNode methodCodeNode)
        {
            _context = new CodeGeneratorContext(localVariableTable, methodCodeNode, _configuration, _nameMangler);

            AssignFrameOffsets();

            var methodInstructions = new List<Instruction>();

            GenerateStringMap(blocks);
            GenerateStringData(_context.Emitter);

            _context.Emitter.CreateLabel(_nameMangler.GetMangledMethodName(_context.Method));

            GenerateProlog(_context.Emitter);
            methodInstructions.AddRange(_context.Emitter.Instructions);

            foreach (var block in blocks)
            {
                _context.Emitter.Reset();

                _context.Emitter.CreateLabel(block.Label);

                var visitorAdapter = new GenericStackEntryAdapter(this);

                var currentNode = block.FirstNode;
                while (currentNode != null)
                {
                    currentNode.Accept(visitorAdapter);
                    currentNode = currentNode.Next;
                }

                Optimize(_context.Emitter.Instructions);
                methodInstructions.AddRange(_context.Emitter.Instructions);
            }

            return methodInstructions;
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

        // TODO: Consider making this a separate phase
        private void GenerateStringMap(IList<BasicBlock> blocks)
        {
            // Process all nodes in the basic blocks and extract string definitions to populate the string map
            foreach (var block in blocks)
            {
                var currentNode = block.FirstNode;
                while (currentNode != null)
                {
                    if (currentNode is StringConstantEntry)
                    {
                        var stringConstantEntry = currentNode.As<StringConstantEntry>();

                        var label = LabelGenerator.GetLabel(LabelType.String);
                        _labelsToStringData[label] = stringConstantEntry.Value;

                        stringConstantEntry.Label = label;
                    }
                    currentNode = currentNode.Next;
                }
            }
        }

        private void GenerateStringData(Emitter emitter)
        {
            var systemStringType = _corLibModuleProvider.FindThrow("System.String");
            var systemStringEETypeMangledName = _nameMangler.GetMangledTypeName(systemStringType);

            // TODO: Need to eliminate duplicate strings
            foreach (var keyValuePair in _labelsToStringData)
            {
                emitter.CreateLabel(keyValuePair.Key);

                var stringData = keyValuePair.Value;

                emitter.Dw(systemStringEETypeMangledName);

                byte lsb = (byte)(stringData.Length & 0xFF);
                byte msb = (byte)((stringData.Length >> 8) & 0xFF);

                emitter.Db(lsb);
                emitter.Db(msb);

                foreach (var ch in stringData)
                {
                    emitter.Db((byte)ch);
                    emitter.Db((byte)0x00);
                }
            }
        }
        private void GenerateProlog(Emitter emitter)
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

            if (_context.ParamsCount > 0 || (_context.LocalsCount + tempCount) > 0)
            {
                emitter.Push(IX);
                emitter.Ld(IX, 0);
                emitter.Add(IX, SP);
            }

            if (_context.LocalsCount + tempCount > 0)
            {
                // Reserve space on stack for locals
                emitter.Ld(HL, (short)-localsSize);
                emitter.Add(HL, SP);
                emitter.Ld(SP, HL);
                
                if (_context.Method.Body.InitLocals)
                {
                    /*
                    // TODO: This should loop through the locals and only init those flagged as must init
                    // but this requires SSA and use/def analysis which we don't have yet.
                    // So for now just init all the locals

                    emitter.Ld(Bc, (short)localsSize);

                    emitter.Push(Ix);
                    emitter.Pop(Hl);

                    var initLoopLabel = _context.NameMangler.GetUniqueName();
                    emitter.CreateLabel(initLoopLabel);

                    emitter.Dec(Hl);  // Stack grows downwards so need to move to next byte first

                    emitter.Ld(__[Hl], 0);

                    emitter.Dec(Bc);

                    emitter.Ld(A, B);
                    emitter.Or(C);

                    emitter.Jp(Condition.NZ, initLoopLabel);
                    */
                }
            }
        }

        private void Optimize(IList<Instruction> instructions)
        {
            EliminatePushXXPopXX(instructions);
        }

        private void EliminatePushXXPopXX(IList<Instruction> instructions)
        {
            int unoptimizedInstructionCount = instructions.Count;
            Instruction? lastInstruction = null;
            var currentInstruction = instructions[0];
            int count = 0;
            do
            {
                if (lastInstruction?.Opcode == Opcode.Push && currentInstruction.Opcode == Opcode.Pop
                    && lastInstruction?.Op0?.Register == currentInstruction.Op0?.Register)
                {
                    // Eliminate Push followed by Pop
                    instructions.RemoveAt(count - 1);
                    instructions.RemoveAt(count - 1);

                    count--;
                    currentInstruction = instructions[count];
                    lastInstruction = count > 0 ? instructions[count - 1] : null;
                }
                else
                {
                    lastInstruction = currentInstruction;
                    if (count + 1 < instructions.Count)
                    {
                        currentInstruction = instructions[++count];
                    }
                }
            } while (count < instructions.Count - 1);

            _logger.LogDebug("Eliminated {eliminatedInstructions} instructions", unoptimizedInstructionCount - instructions.Count);
        }
    }
}
