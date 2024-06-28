﻿using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.DependencyAnalysisFramework;
using ILCompiler.Compiler.Emit;
using ILCompiler.Interfaces;
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
                return 0x0000;
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
            instructionsBuilder.Ld(SP, (short)_configuration.StackStart);

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

            instructionsBuilder.Label("START");

            instructionsBuilder.Call(_nameMangler.GetMangledMethodName(entryMethod));

            instructionsBuilder.Label("EH_ENDIP");
            instructionsBuilder.Jp("EXITRETCODE");

            _out.Write(instructionsBuilder.ToString());
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

        public void WriteCode(Z80MethodCodeNode rootNode, IReadOnlyCollection<IDependencyNode> nodes, string inputFilePath, string outputFilePath)
        {
            _inputFilePath = inputFilePath;

            using (_out = new StreamWriter(new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, false), Encoding.ASCII))
            {
                WriteProlog(rootNode);

                foreach (var node in nodes)
                {
                    if (!node.ShouldSkipEmitting(_nodeFactory))
                    {
                        var instructions = node.GetInstructions(inputFilePath);
                        WriteInstructions(instructions);
                    }
                }

                WriteEhClauses(nodes);

                WriteEpilog();
            }

            _logger.LogDebug("Written compiled file to {_outputFilePath}", outputFilePath);
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
