using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.Emit;
using ILCompiler.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text;

namespace ILCompiler.Compiler
{
    public class Z80Writer
    {
        private readonly IConfiguration _configuration;
        private readonly INameMangler _nameMangler;
        private readonly ILogger<Z80Writer> _logger;
        private readonly NativeDependencyAnalyser _nativeDependencyAnalyser;

        private string _inputFilePath = null!;
        private string _outputFilePath = null!;
        private StreamWriter _out = null!;

        private readonly ISet<string> _calls = new HashSet<string>();

        public Z80Writer(IConfiguration configuration, INameMangler nameMangler, ILogger<Z80Writer> logger, NativeDependencyAnalyser nativeDependencyAnalyser)
        {
            _configuration = configuration;
            _nameMangler = nameMangler;
            _logger = logger;
            _nativeDependencyAnalyser = nativeDependencyAnalyser;
        }

        private void OutputMethodNode(Z80MethodCodeNode methodCodeNode)
        {
            if (methodCodeNode.MethodCode != null)
            {
                _out.Write(methodCodeNode.MethodCode);
                _calls.UnionWith(NativeDependencyAnalyser.GetMethodCalls(methodCodeNode.MethodCode));
            }
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
                TargetArchitecture.ZXSpectrum => 0x8000,
                _ => throw new ArgumentException("Invalid target architecture value"),
            };
        }

        private const string Entry = "ENTRY";
        private const string Heap = "HEAP";

        private void OutputProlog(MethodDesc entryMethod)
        {
            _out.WriteLine($"; INPUT FILE {_inputFilePath.ToUpper()}");
            _out.WriteLine($"; {DateTime.Now}");
            _out.WriteLine();

            _out.WriteLine(Instruction.Org(GetOrgAddress()));
            OutputLabel(Entry);

            _out.WriteLine(Instruction.Ld(R16.HL, Heap));
            _out.WriteLine(Instruction.LdInd("HEAPNEXT", R16.HL));

            // Save original stack location
            _out.WriteLine(Instruction.LdInd("ORIGSP", R16.SP));

            // Relocate the stack
            _out.WriteLine(Instruction.Ld(R16.SP, (short)_configuration.StackStart));

            // Start the program
            _out.WriteLine(Instruction.Jp("START"));

            // Restore the original stack and return to the OS
            OutputLabel("EXITRETCODE");

            var returnType = entryMethod.ReturnType;
            var hasReturnCode = returnType != null && returnType.GetVarType().IsInt();

            if (!_configuration.IntegrationTests && hasReturnCode)
            {
                OutputReturnCodeHandler();
            }

            OutputLabel("EXIT");
            if (!_configuration.IntegrationTests)
            { 
                // Reset stack pointer and return to OS
                _out.WriteLine(Instruction.LdInd(R16.SP, "ORIGSP"));
                _out.WriteLine(Instruction.Ret());
            }
            else
            {
                // Move return code to HL/DE and Halt
                _out.WriteLine(Instruction.Pop(R16.DE));
                _out.WriteLine(Instruction.Pop(R16.HL));
                _out.WriteLine(Instruction.Halt());
            }

            // Holds the original stack location
            _out.WriteLine(Instruction.Db("  ", "ORIGSP"));

            // Holds next free heap location
            _out.WriteLine(Instruction.Db("  ", "HEAPNEXT"));

            // Output messages
            OutputOOMMessage();
            if (hasReturnCode && _configuration.PrintReturnCode)
            {
                OutputReturnCodeMessage();
            }

            // Include the runtime assembly code
            if (_configuration.DontInlineRuntime)
            {
                _out.WriteLine("include csharprt.asm");

                switch (_configuration.TargetArchitecture)
                {
                    case TargetArchitecture.TRS80: _out.WriteLine("include trs80rt.asm"); break;
                    case TargetArchitecture.CPM: _out.WriteLine("include cpmrt.asm"); break;
                    case TargetArchitecture.ZXSpectrum: _out.WriteLine("include zxspectrum.asm"); break;
                    default: throw new ArgumentException("Invalid target architecture value");
                }
            }

            OutputLabel("START");
            // TODO: Call static constructors here
            _out.WriteLine(Instruction.Call(_nameMangler.GetMangledMethodName(entryMethod)));
            _out.WriteLine(Instruction.Jp("EXITRETCODE"));
        }

        private void OutputOOMMessage()
        {
            OutputLabel("OOM_MSG");
            _out.WriteLine(Instruction.Db(13));
            _out.WriteLine(Instruction.Db(0)); // Length of message
            _out.WriteLine(Instruction.Db("O u t   o f   m e m o r y "));
        }

        private void OutputReturnCodeMessage()
        {
            OutputLabel("RETCODEMSG");
            _out.WriteLine(Instruction.Db(12));
            _out.WriteLine(Instruction.Db(0)); // Length of message
            _out.WriteLine(Instruction.Db("R e t u r n   C o d e : "));
        }

        private void OutputLabel(string label) => _out.WriteLine(new LabelInstruction(label));

        private void OutputReturnCodeHandler()
        {
            if (_configuration.PrintReturnCode)
            {
                // Write string "Return Code:"
                _out.WriteLine(Instruction.Ld(R16.HL, "RETCODEMSG - 2"));
                _out.WriteLine(Instruction.Call("PRINT"));
                _calls.Add("PRINT");

                // Write return code
                _out.WriteLine(Instruction.Pop(R16.HL));
                _out.WriteLine(Instruction.Pop(R16.DE));
                _out.WriteLine(Instruction.Call("LTOA"));
                _calls.Add("LTOA");
            }
            else
            {
                if (_configuration.TargetArchitecture == TargetArchitecture.ZXSpectrum)
                {
                    // Remove return value as not supported on ZX spectrum
                    _out.WriteLine(Instruction.Pop(R16.BC));
                    _out.WriteLine(Instruction.Pop(R16.DE));
                }
                else
                {
                    _out.WriteLine(Instruction.Pop(R16.BC));    // return value
                    _out.WriteLine(Instruction.Pop(R16.DE));
                    _out.WriteLine(Instruction.Pop(R16.HL));    // return address
                    _out.WriteLine(Instruction.Push(R16.DE));
                    _out.WriteLine(Instruction.Push(R16.BC));
                    _out.WriteLine(Instruction.Push(R16.HL));
                }
            }
        }

        private void OutputEpilog()
        {
            if (!_configuration.DontInlineRuntime)
            {
                OutputRuntimeCode();
            }

            _out.WriteLine();

            OutputLabel(Heap);
            _out.WriteLine(Instruction.End(Entry));
        }

        private void OutputCodeForNode(Z80MethodCodeNode node)
        {
            if (!node.CodeEmitted)
            {
                node.CodeEmitted = true;
                if (node.MethodCode != null)
                {
                    _out.WriteLine($"; {node.Method.FullName}");
                    OutputMethodNode(node);
                }

                if (node.Dependencies != null)
                {
                    foreach (var dependentNode in node.Dependencies)
                    {
                        if (dependentNode is Z80MethodCodeNode z80MethodCodeNode)
                        {
                            OutputCodeForNode(z80MethodCodeNode);
                        }
                    }
                }
            }
        }

        public void OutputCode(Z80MethodCodeNode root, string inputFilePath, string outputFilePath)
        {
            _inputFilePath = inputFilePath;
            _outputFilePath = outputFilePath;

            using (_out = new StreamWriter(new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, false), Encoding.ASCII))
            {
                OutputProlog(root.Method);

                OutputNodes(root);
                OutputCodeForNode(root);

                OutputEpilog();
            }

            _logger.LogDebug("Written compiled file to {_outputFilePath}", _outputFilePath);
        }

        private void OutputNodes(Z80MethodCodeNode node)
        {
            var dependencies = DependencyNodeHelpers.GetFlattenedDependencies(node);
            foreach (var dependentNode in dependencies)
            {
                // If the type has static fields then need to reserve space for these fields
                if (dependentNode is EETypeNode typeNode)
                {
                    var typeDef = typeNode.Type;
                    foreach (var field in typeDef.Fields)
                    {
                        if (field.IsStatic)
                        {
                            var fieldSize = field.FieldType.GetInstanceFieldSize();
                            _logger.LogDebug("Reserving {fieldSize} bytes for static field {field.FullName}", fieldSize, field.FullName);

                            // Need to mangle full field name here
                            OutputLabel(_nameMangler.GetMangledFieldName(field));

                            // Emit fieldSize bytes with value 0
                            _out.WriteLine(Instruction.Dc(fieldSize, 0));
                        }
                    }
                }
            }
        }

        private void OutputRuntimeCode()
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
