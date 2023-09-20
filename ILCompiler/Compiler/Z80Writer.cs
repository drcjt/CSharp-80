using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.Emit;
using ILCompiler.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text;
using static ILCompiler.Compiler.Emit.Registers;

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
                TargetArchitecture.ZXSpectrum => 0x5CCB,
                _ => throw new ArgumentException("Invalid target architecture value"),
            };
        }

        private const string Entry = "ENTRY";
        private const string Heap = "HEAP";

        private void OutputProlog(Z80MethodCodeNode root)
        {
            _out.WriteLine($"; INPUT FILE {_inputFilePath.ToUpper()}");
            _out.WriteLine($"; {DateTime.Now}");
            _out.WriteLine();

            var emitter = new Emitter();

            emitter.Org(GetOrgAddress());
            emitter.CreateLabel(Entry);

            emitter.Ld(HL, Heap);
            emitter.Ld(__["HEAPNEXT"], HL);

            // Save original stack location
            emitter.Ld(__["ORIGSP"], SP);

            // Relocate the stack
            emitter.Ld(SP, (short)_configuration.StackStart);

            // Start the program
            emitter.Jp("START");

            // Restore the original stack and return to the OS
            emitter.CreateLabel("EXITRETCODE");

            var entryMethod = root.Method;
            var returnType = entryMethod.ReturnType;
            var hasReturnCode = returnType != null && returnType.GetVarType().IsInt();

            if (!_configuration.IntegrationTests && hasReturnCode)
            {
                OutputReturnCodeHandler(emitter);
            }

            emitter.CreateLabel("EXIT");
            if (!_configuration.IntegrationTests)
            {
                if (_configuration.TargetArchitecture == TargetArchitecture.ZXSpectrum)
                {
                    // No point exiting so just loop forever
                    emitter.CreateLabel("EXITLOOP");
                    emitter.Jp("EXITLOOP");
                }
                else
                {
                    // Reset stack pointer and return to OS
                    emitter.Ld(SP, __["ORIGSP"]);
                    emitter.Ret();
                }
            }
            else
            {
                // Move return code to HL/DE and Halt
                emitter.Pop(DE);
                emitter.Pop(HL);
                emitter.Halt();
            }

            // Holds the original stack location
            emitter.Db("ORIGSP", "  ");

            // Holds next free heap location
            emitter.Db("HEAPNEXT", "  ");

            // Output messages
            OutputOOMMessage(emitter);
            if (hasReturnCode && _configuration.PrintReturnCode)
            {
                OutputReturnCodeMessage(emitter);
            }
            OutputIndexOutOfRangeMessage(emitter);

            emitter.CreateLabel("START");

            emitter.Call(_nameMangler.GetMangledMethodName(entryMethod));
            emitter.Jp("EXITRETCODE");

            _out.WriteLine(emitter.ToString());
        }

        private static IList<IDependencyNode> _nodesProcessed = new List<IDependencyNode>();
        private void OutputStaticConstructorInitialization(IDependencyNode node, Emitter emitter)
        {
            if (!_nodesProcessed.Contains(node))
            {
                _nodesProcessed.Add(node);

                // Process dependent nodes first
                foreach (var dependency in node.Dependencies)
                {
                    if (dependency is Z80MethodCodeNode dependentMethodNode)
                    {
                        OutputStaticConstructorInitialization(dependentMethodNode, emitter);
                    }
                }

                if (node is Z80MethodCodeNode methodNode && methodNode.Method.IsStaticConstructor)
                {
                    // Now output call to the static constructor
                    emitter.Call(_nameMangler.GetMangledMethodName(methodNode.Method));
                }
            }
        }

        private static void OutputOOMMessage(Emitter emitter)
        {
            emitter.CreateLabel("OOM_MSG");
            emitter.Db(13);
            emitter.Db(0); // Length of message
            emitter.Db("O u t   o f   m e m o r y ");
        }

        private static void OutputReturnCodeMessage(Emitter emitter)
        {
            emitter.CreateLabel("RETCODEMSG");
            emitter.Db(12);
            emitter.Db(0); // Length of message
            emitter.Db("R e t u r n   C o d e : ");
        }

        private static void OutputIndexOutOfRangeMessage(Emitter emitter)
        {
            emitter.CreateLabel("INDEX_OUT_OF_RANGE_MSG");
            emitter.Db(18);
            emitter.Db(0); // Length of message
            emitter.Db("I n d e x   O u t   O f   R a n g e ");
        }

        private void OutputLabel(string label) => _out.WriteLine(Instruction.Create(label));

        private void OutputReturnCodeHandler(Emitter emitter)
        {
            if (_configuration.PrintReturnCode)
            {
                // Write string "Return Code:"
                emitter.Ld(HL, "RETCODEMSG - 2");
                emitter.Call("PRINT");
                _calls.Add("PRINT");

                // Write return code
                emitter.Pop(HL);
                emitter.Pop(DE);
                emitter.Call("LTOA");
                _calls.Add("LTOA");
            }
            else
            {
                if (_configuration.TargetArchitecture == TargetArchitecture.ZXSpectrum)
                {
                    // Remove return value as not supported on ZX spectrum
                    emitter.Pop(BC);
                    emitter.Pop(DE);
                }
                else
                {
                    emitter.Pop(BC);    // return value
                    emitter.Pop(DE);
                    emitter.Pop(HL);    // return address
                    emitter.Push(DE);
                    emitter.Push(BC);
                    emitter.Push(HL);
                }
            }
        }

        private void OutputEpilog()
        {
            OutputRuntimeCode();

            _out.WriteLine();

            var emitter = new Emitter();
            emitter.CreateLabel(Heap);
            emitter.End(Entry);

            _out.Write(emitter.ToString());
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
                OutputProlog(root);

                OutputStatics(root);
                OutputEETypes(root);
                OutputCodeForNode(root);

                OutputEpilog();
            }

            _logger.LogDebug("Written compiled file to {_outputFilePath}", _outputFilePath);
        }

        private void OutputStatics(Z80MethodCodeNode node)
        {
            var dependencies = DependencyNodeHelpers.GetFlattenedDependencies(node);
            foreach (var dependentNode in dependencies)
            {
                // For static field dependencies we need to reserve appropriate space for the field
                if (dependentNode is StaticsNode typeNode)
                {
                    var field = typeNode.Field;
                    var fieldSize = field.FieldType.GetInstanceFieldSize();
                    _logger.LogDebug("Reserving {fieldSize} bytes for static field {field.FullName}", fieldSize, field.FullName);

                    // Need to mangle full field name here
                    OutputLabel(_nameMangler.GetMangledFieldName(field));

                    // Emit fieldSize bytes with value 0
                    _out.WriteLine(Instruction.Create(Opcode.Dc, (ushort)fieldSize, 0));

                }
            }
        }

        private void OutputEETypes(Z80MethodCodeNode node)
        {
            var eeTypes = new List<string>();

            var dependencies = DependencyNodeHelpers.GetFlattenedDependencies(node);
            foreach (var dependentNode in dependencies)
            {
                // For static field dependencies we need to reserve appropriate space for the field
                if (dependentNode is ConstructedEETypeNode typeNode)
                {
                    var eeMangledTypeName = _nameMangler.GetMangledTypeName(typeNode.Type);

                    if (!eeTypes.Contains(eeMangledTypeName))
                    {
                        eeTypes.Add(eeMangledTypeName);

                        _out.WriteLine($";{typeNode.Type.FullName}");
                        // Need to mangle full field name here
                        OutputLabel(eeMangledTypeName);

                        // Emit data for EEType here
                        var baseSize = typeNode.BaseSize;

                        byte lsb = (byte)(baseSize & 0xFF);
                        byte msb = (byte)((baseSize >> 8) & 0xFF);

                        _out.WriteLine(Instruction.Create(Opcode.Db, lsb));
                        _out.WriteLine(Instruction.Create(Opcode.Db, msb));

                        if (typeNode.RelatedType != null)
                        {
                            var relatedTypeMangledTypeName = _nameMangler.GetMangledTypeName(typeNode.RelatedType);
                            _out.WriteLine(Instruction.Create(Opcode.Dw, relatedTypeMangledTypeName));
                        }
                        else
                        {
                            _out.WriteLine(Instruction.Create(Opcode.Dw, (ushort)0));
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
