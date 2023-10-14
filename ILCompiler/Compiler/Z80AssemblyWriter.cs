using ILCompiler.Compiler.DependencyAnalysis;
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

        private string _inputFilePath = null!;
        private StreamWriter _out = null!;

        private readonly ISet<string> _calls = new HashSet<string>();

        public Z80AssemblyWriter(IConfiguration configuration, INameMangler nameMangler, ILogger<Z80AssemblyWriter> logger, NativeDependencyAnalyser nativeDependencyAnalyser)
        {
            _configuration = configuration;
            _nameMangler = nameMangler;
            _logger = logger;
            _nativeDependencyAnalyser = nativeDependencyAnalyser;
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
            var returnType = entryMethod.ReturnType;
            var hasReturnCode = returnType != null && returnType.GetVarType().IsInt();

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
            WriteIndexOutOfRangeMessage(instructionsBuilder);

            instructionsBuilder.Label("START");

            instructionsBuilder.Call(_nameMangler.GetMangledMethodName(entryMethod));
            instructionsBuilder.Jp("EXITRETCODE");

            _out.WriteLine(instructionsBuilder.ToString());
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

        private static void WriteIndexOutOfRangeMessage(InstructionsBuilder instructionsBuilder)
        {
            instructionsBuilder.Label("INDEX_OUT_OF_RANGE_MSG");
            instructionsBuilder.Db(18);
            instructionsBuilder.Db(0); // Length of message
            instructionsBuilder.Db("I n d e x   O u t   O f   R a n g e ");
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
                    var instructions = node.GetInstructions(inputFilePath);
                    WriteInstructions(instructions);
                }

                WriteEpilog();
            }

            _logger.LogDebug("Written compiled file to {_outputFilePath}", outputFilePath);
        }

        private void WriteInstructions(IList<Emit.Instruction> instructions)
        {
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
