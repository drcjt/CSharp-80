﻿using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.DependencyAnalysisFramework;
using ILCompiler.Compiler.Emit;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using Microsoft.Extensions.Logging;
using System.Text;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler
{
    public class Z80AssemblyWriter
    {
        private readonly IConfiguration _configuration;
        private readonly INameMangler _nameMangler;
        private readonly ILogger<Z80AssemblyWriter> _logger;
        private readonly NativeDependencyAnalyser _nativeDependencyAnalyser;
        private readonly NodeFactory _nodeFactory;

        private string _inputFilePath = null!;
        private StreamWriter _out = null!;

        private readonly ISet<string> _calls = new HashSet<string>();

        public Z80AssemblyWriter(IConfiguration configuration, INameMangler nameMangler, ILogger<Z80AssemblyWriter> logger, NativeDependencyAnalyser nativeDependencyAnalyser, NodeFactory nodeFactory)
        {
            _configuration = configuration;
            _nameMangler = nameMangler;
            _logger = logger;
            _nativeDependencyAnalyser = nativeDependencyAnalyser;
            _nodeFactory = nodeFactory;
        }

        private ushort GetOrgAddress()
        {
            if (_configuration.IntegrationTests)
            {
                return 0x0050;
            }
            return _configuration.TargetArchitecture switch
            {
                TargetArchitecture.TRS80 => 0x5200,
                TargetArchitecture.CPM => 0x100,
                TargetArchitecture.ZXSpectrum => 0x5CCB,
                _ => throw new ArgumentException("Invalid target architecture value"),
            };
        }

        private const string Entry = "ENTRY";
        private const string Heap = "HEAP";

        private void WriteProlog(Z80MethodCodeNode root)
        {
            _out.WriteLine($"; INPUT FILE {_inputFilePath.ToUpper()}");
            _out.WriteLine($"; {DateTime.Now}");
            _out.WriteLine();

            var instructionsBuilder = new InstructionsBuilder();

            instructionsBuilder.Org(GetOrgAddress());
            instructionsBuilder.Label(Entry);

            instructionsBuilder.Ld(HL, Heap);
            instructionsBuilder.Ld(__["HEAPNEXT"], HL);

            // Save original stack location
            instructionsBuilder.Ld(__["ORIGSP"], SP);

            // Relocate the stack
            RelocateStack(instructionsBuilder);

            // Start the program
            instructionsBuilder.Jp("START");

            // Restore the original stack and return to the OS
            instructionsBuilder.Label("EXITRETCODE");

            var entryMethod = root.Method;
            var returnType = entryMethod.Signature.ReturnType;
            var hasReturnCode = returnType != null && returnType.VarType.IsInt();

            if (!_configuration.IntegrationTests && hasReturnCode)
            {
                WriteReturnCodeHandler(instructionsBuilder);
            }

            instructionsBuilder.Label("EXIT");
            if (!_configuration.IntegrationTests)
            {
                if (_configuration.TargetArchitecture == TargetArchitecture.ZXSpectrum)
                {
                    // No point exiting so just loop forever
                    instructionsBuilder.Label("EXITLOOP");
                    instructionsBuilder.Jp("EXITLOOP");
                }
                else
                {
                    // Reset stack pointer and return to OS
                    instructionsBuilder.Ld(SP, __["ORIGSP"]);
                    instructionsBuilder.Ret();
                }
            }
            else
            {
                // Move return code to HL/DE and Halt
                instructionsBuilder.Pop(DE);
                instructionsBuilder.Pop(HL);
                instructionsBuilder.Halt();
            }

            // Holds the original stack location
            instructionsBuilder.Db("ORIGSP", "  ");

            // Holds next free heap location
            instructionsBuilder.Db("HEAPNEXT", "  ");

            // Output messages
            WriteOOMMessage(instructionsBuilder);
            if (hasReturnCode && _configuration.PrintReturnCode)
            {
                WriteReturnCodeMessage(instructionsBuilder);
            }

            DefType systemArrayType = entryMethod.Context.GetWellKnownType(WellKnownType.Array)!;
            if (_nodeFactory.ConstructedEETypeNodeDefined(systemArrayType))
            {
                var systemArrayTypeLabel = _nameMangler.GetMangledTypeName(systemArrayType);
                instructionsBuilder.Equ("SYSTEMARRAY", systemArrayTypeLabel);
            }
            else
            {
                instructionsBuilder.Equ("SYSTEMARRAY", 0);
            }

            instructionsBuilder.Label("START");

            LoadMainArguments(instructionsBuilder, entryMethod);

            instructionsBuilder.Call(_nameMangler.GetMangledMethodName(entryMethod));

            instructionsBuilder.Label("EH_ENDIP");
            instructionsBuilder.Jp("EXITRETCODE");

            _out.Write(instructionsBuilder.ToString());
        }

        private static void LoadMainArguments(InstructionsBuilder instructionsBuilder, MethodDesc entryMethod)
        {
            // If Main method has string[] args then need to push reference on the stack here
            if (entryMethod.Parameters.Count > 0)
            {
                // Push a null reference for the args array
                instructionsBuilder.Ld(HL, 0);
                instructionsBuilder.Push(HL);
            }
        }

        private void RelocateStack(InstructionsBuilder instructionsBuilder)
        {
            if (_configuration.StackStart != null)
            {
                instructionsBuilder.Ld(SP, (short)_configuration.StackStart);
            }
            else
            {
                if (_configuration.TargetArchitecture == TargetArchitecture.TRS80)
                {
                    instructionsBuilder.Ld(HL, __[0x40B1]); // Get MEMSIZ value
                    instructionsBuilder.Ld(SP, HL);
                }
                else
                {
                    instructionsBuilder.Ld(SP, 0xa000);
                }
            }
        }

        private static void WriteOOMMessage(InstructionsBuilder instructionsBuilder)
        {
            instructionsBuilder.Label("OOM_MSG");
            instructionsBuilder.Db(13);
            instructionsBuilder.Db(0); // Length of message
            instructionsBuilder.Db("O u t   o f   m e m o r y ");
        }

        private static void WriteReturnCodeMessage(InstructionsBuilder instructionsBuilder)
        {
            instructionsBuilder.Label("RETCODEMSG");
            instructionsBuilder.Db(12);
            instructionsBuilder.Db(0); // Length of message
            instructionsBuilder.Db("R e t u r n   C o d e : ");
        }

        private void WriteReturnCodeHandler(InstructionsBuilder instructionsBuilder)
        {
            if (_configuration.PrintReturnCode)
            {
                // Write string "Return Code:"
                instructionsBuilder.Ld(HL, "RETCODEMSG - 2");
                instructionsBuilder.Call("PRINT");
                _calls.Add("PRINT");

                // Write return code
                instructionsBuilder.Pop(HL);
                instructionsBuilder.Pop(DE);
                instructionsBuilder.Call("LTOA");
                _calls.Add("LTOA");
            }
            else
            {
                if (_configuration.TargetArchitecture == TargetArchitecture.ZXSpectrum)
                {
                    // Remove return value as not supported on ZX spectrum
                    instructionsBuilder.Pop(BC);
                    instructionsBuilder.Pop(DE);
                }
                else
                {
                    instructionsBuilder.Pop(BC);    // return value
                    instructionsBuilder.Pop(DE);
                    instructionsBuilder.Pop(HL);    // return address
                    instructionsBuilder.Push(DE);
                    instructionsBuilder.Push(BC);
                    instructionsBuilder.Push(HL);
                }
            }
        }

        private void WriteEpilog()
        {
            WriteRuntimeCode();

            _out.WriteLine();

            var instructionsBuilder = new InstructionsBuilder();
            instructionsBuilder.Label(Heap);
            instructionsBuilder.End(Entry);

            _out.Write(instructionsBuilder.ToString());
        }

        private void WritePInvokeModules(IList<string> pinvokeModules)
        {
            foreach (var module in pinvokeModules)
            {
                _out.WriteLine();
                _out.WriteLine($"; External file: {module}");
                _out.WriteLine();
                _out.Write(File.ReadAllText(module));
                _out.WriteLine();
            }
        }

        public static void FindAllCalledMethods(IList<Emit.Instruction> instructions, ISet<string> referencedMethods)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.Opcode == Emit.Opcode.Call && instruction.Op0 is not null)
                {
                    var target = instruction.Op0.Label;
                    if (target is not null)
                    {
                        referencedMethods.Add(target);
                    }
                }
            }
        }

        private readonly Dictionary<IDependencyNode, IList<Instruction>> _instructionsCache = [];

        public IList<Instruction> GetInstructions(IDependencyNode node, string inputFilePath, List<string> modules)
        {
            if (!_instructionsCache.TryGetValue(node, out var instructions))
            {
                instructions = node.GetInstructions(inputFilePath, modules);
                _instructionsCache[node] = instructions;
            }
            return instructions;
        }

        private static bool IsMethodTrimmable(Z80MethodCodeNode node) =>
            !node.Method.IsVirtual &&
            node.Method.Name != "ThrowException" &&
            !node.Method.IsIntrinsic &&
            !node.Method.IsInternalCall &&
            !node.Method.IsPInvoke;

        public void WriteCode(Z80MethodCodeNode rootNode, IReadOnlyCollection<IDependencyNode> nodes, string inputFilePath, string outputFilePath)
        {
            _inputFilePath = inputFilePath;

            var modules = new List<string>();

            var allReferencedMethods = new HashSet<string>();
            foreach (var node in nodes)
            {
                if (!node.ShouldSkipEmitting(_nodeFactory))
                {
                    var instructions = GetInstructions(node, inputFilePath, modules);
                    FindAllCalledMethods(instructions, allReferencedMethods);
                }
            }

            var methodsRequiringUnwindInfo = new List<Z80MethodCodeNode>();

            using (_out = new StreamWriter(new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, false), Encoding.ASCII))
            {
                WriteProlog(rootNode);

                foreach (var node in nodes)
                {
                    if (!node.ShouldSkipEmitting(_nodeFactory))
                    {
                        if (node != rootNode && node is Z80MethodCodeNode codeNode && IsMethodTrimmable(codeNode))
                        {
                            var mangledName = codeNode.GetMangledName(_nameMangler);
                            if (!allReferencedMethods.Contains(mangledName))
                            {
                                _logger.LogInformation("Skipping method {MethodName} as not referenced", codeNode.Method.FullName);
                                continue;
                            }
                        }

                        var instructions = GetInstructions(node, inputFilePath, modules);
                        WriteInstructions(instructions);

                        if (instructions.Count > 0 && node is Z80MethodCodeNode methodCodeNode)
                        {
                            methodsRequiringUnwindInfo.Add(methodCodeNode);
                        }
                    }
                }

                WriteUnwindTable(methodsRequiringUnwindInfo);

                WriteEhClauses(nodes);

                WritePInvokeModules(modules);

                WriteEpilog();
            }

            _logger.LogDebug("Written compiled file to {_outputFilePath}", outputFilePath);
        }

        private void WriteUnwindTable(IReadOnlyCollection<Z80MethodCodeNode> nodes)
        {
            if (Compilation.AnyExceptionHandlers)
            {
                var mostUsedNumberOfParameters = nodes.GroupBy(node => node.ParameterBytes)
                                                      .OrderByDescending(g => g.Count())
                                                      .Select(g => g.Key)
                                                      .First();

                InstructionsBuilder unwindBuilder = new InstructionsBuilder();
                unwindBuilder.Label("UNWIND_TABLE");
                foreach (var node in nodes)
                {
                    if (node.Method.Name != "ThrowException" && node.ParameterBytes != mostUsedNumberOfParameters)
                    {
                        var mangledName = node.GetMangledName(_nameMangler);

                        unwindBuilder.Dw(mangledName, "Method Start");
                        unwindBuilder.Dw($"{mangledName}_END", "Method End");
                        unwindBuilder.Db(node.ParameterBytes, "Unwind Parameter Bytes");
                    }
                }

                unwindBuilder.Dw(0, "Catch all entry start");
                unwindBuilder.Dw(0xFFFF, "Catch all entry end");
                unwindBuilder.Db(mostUsedNumberOfParameters, "Catch all unwind parameter bytes");

                unwindBuilder.Label("UNWIND_TABLE_END");
                WriteInstructions(unwindBuilder.Instructions);
            }
        }

        private void WriteEhClauses(IReadOnlyCollection<IDependencyNode> nodes)
        {
            if (Compilation.AnyExceptionHandlers)
            {
                InstructionsBuilder ehClausesBuilder = new InstructionsBuilder();
                ehClausesBuilder.Label("EH_CLAUSES");
                foreach (var node in nodes)
                {
                    if (node is Z80MethodCodeNode codeNode)
                    {
                        var ehClauses = codeNode.EhClauses;

                        if (ehClauses.Count > 0)
                        {
                            ehClausesBuilder.Comment($"{codeNode.Method.FullName} EH Clauses");
                            foreach (var ehClause in ehClauses)
                            {
                                if (ehClause.Kind == EHClauseKind.Typed)
                                {
                                    ehClausesBuilder.Dw(ehClause.TryBegin.Label, "Protected Region Start");
                                    if (ehClause.TryEnd != null)
                                    {
                                        ehClausesBuilder.Dw(ehClause.TryEnd.Label, "Protected Region End");
                                    }
                                    else
                                    {
                                        var methodName = codeNode.GetMangledName(_nameMangler);
                                        ehClausesBuilder.Dw($"{methodName}_END", "Protected Region End");
                                    }
                                    ehClausesBuilder.Dw(ehClause.HandlerBegin.Label, "Handler Start");
                                    ehClausesBuilder.Dw(ehClause.CatchTypeMangledName, "Catch Type");
                                }
                            }
                        }
                    }
                }
                ehClausesBuilder.Label("EH_CLAUSES_END");
                WriteInstructions(ehClausesBuilder.Instructions);
            }
        }

        private void WriteInstructions(IList<Emit.Instruction> instructions)
        {
            _out.WriteLine();

            var stringBuilder = new StringBuilder();
            foreach (var instruction in instructions)
            {
                stringBuilder.AppendLine(instruction.ToString());
            }
            _calls.UnionWith(NativeDependencyAnalyser.GetMethodCalls(stringBuilder.ToString()));

            _out.Write(stringBuilder.ToString());
        }

        private void WriteRuntimeCode()
        {
            _out.WriteLine();
            _out.WriteLine("; **** Runtime starts here");

            var nativeResourceNames = _nativeDependencyAnalyser.GetNativeResourceNames(_calls);
            foreach (string resourceName in nativeResourceNames)
            {
                using Stream? stream = GetType().Assembly.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    using var reader = new StreamReader(stream);
                    _out.Write(reader.ReadToEnd());
                    _out.WriteLine();
                }
            }
        }
    }
}
