using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Interfaces;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Z80Assembler;

namespace ILCompiler.Compiler
{
    public class Z80Writer
    {
        private readonly IConfiguration _configuration;
        private readonly INameMangler _nameMangler;
        private readonly ILogger<Z80Writer> _logger;

        private string _inputFilePath = null!;
        private string _outputFilePath = null!;
        private StreamWriter _out = null!;

        public Z80Writer(IConfiguration configuration, INameMangler nameMangler, ILogger<Z80Writer> logger)
        {
            _configuration = configuration;
            _nameMangler = nameMangler;
            _logger = logger;
        }

        private void OutputMethodNode(Z80MethodCodeNode methodCodeNode)
        {
            if (methodCodeNode.MethodCode != null)
            {
                _out.Write(methodCodeNode.MethodCode);
            }
        }

        private ushort GetOrgAddress()
        {
            if (_configuration.IntegrationTests)
            {
                return 0x0000;
            }
            switch (_configuration.TargetArchitecture)
            {
                case TargetArchitecture.TRS80: return 0x5200;
                case TargetArchitecture.CPM: return 0x100;
                case TargetArchitecture.ZXSpectrum: return 0x8000;
                default: throw new ArgumentException("Invalid target architecture enum value");
            }
         }

        private void OutputProlog(MethodDesc entryMethod)
        {
            _out.WriteLine($"; INPUT FILE {_inputFilePath.ToUpper()}");
            _out.WriteLine($"; {DateTime.Now}");
            _out.WriteLine();

            _out.WriteLine(Instruction.Org(GetOrgAddress()));
            _out.WriteLine(new LabelInstruction("ENTRY"));

            _out.WriteLine(Instruction.Ld(R16.HL, "HEAP"));
            _out.WriteLine(Instruction.LdInd("HEAPNEXT", R16.HL));

            if (!_configuration.IntegrationTests)
            {
                // Save original stack location
                _out.WriteLine(Instruction.LdInd("ORIGSP", R16.SP));

                // Relocate the stack
                _out.WriteLine(Instruction.Ld(R16.SP, (short)_configuration.StackStart));

                // Start the program
                _out.WriteLine(Instruction.Call("START"));

                // Restore the original stack
                _out.WriteLine(Instruction.LdInd(R16.SP, "ORIGSP"));

                // Return to the operating system
                _out.WriteLine(Instruction.Ret());

                // Holds the original stack location
                _out.WriteLine(Instruction.Db("  ", "ORIGSP"));
            }
            else
            {
                _out.WriteLine(Instruction.Call("START"));
                _out.WriteLine(Instruction.Pop(R16.DE));
                _out.WriteLine(Instruction.Pop(R16.HL));
                _out.WriteLine(Instruction.Halt());
            }

            var returnType = entryMethod.ReturnType;
            var hasReturnCode = returnType != null && returnType.GetVarType().IsInt();

            if (hasReturnCode && _configuration.PrintReturnCode)
            {
                _out.WriteLine(new LabelInstruction("retcodemsg"));
                _out.WriteLine(Instruction.Db(12));
                _out.WriteLine(Instruction.Db(0)); // Length of message
                _out.WriteLine(Instruction.Db("R e t u r n   C o d e : "));
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
                    default: throw new ArgumentException("Invalid target architecture enum value");
                }
            }

            _out.WriteLine(new LabelInstruction("START"));

            // TODO: Call static constructors here

            _out.WriteLine(Instruction.Call(_nameMangler.GetMangledMethodName(entryMethod)));

            if (hasReturnCode)
            {
                if (_configuration.PrintReturnCode)
                {
                    // Write string "Return Code:"
                    _out.WriteLine(Instruction.Ld(R16.HL, "retcodemsg"));
                    _out.WriteLine(Instruction.Call("PRINT"));

                    // Write return code
                    _out.WriteLine(Instruction.Pop(R16.HL));
                    _out.WriteLine(Instruction.Pop(R16.DE));
                    _out.WriteLine(Instruction.Call("LTOA"));
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

            _out.WriteLine(Instruction.Ret());
        }

        private void OutputEpilog()
        {
            if (!_configuration.DontInlineRuntime)
            {
                OutputRuntimeCode();
            }

            _out.WriteLine(new LabelInstruction("HEAP"));

            _out.WriteLine(Instruction.End("ENTRY"));
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
                        if (dependentNode is Z80MethodCodeNode)
                        {
                            OutputCodeForNode((Z80MethodCodeNode)dependentNode);
                        }
                    }
                }
            }
        }

        public void OutputCode(Z80MethodCodeNode root, string inputFilePath, string outputFilePath)
        {
            _inputFilePath = inputFilePath;
            _outputFilePath = outputFilePath;
            _out = new StreamWriter(new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, false), Encoding.ASCII);

            OutputProlog(root.Method);

            OutputNodes(root);

            OutputCodeForNode(root);

            OutputEpilog();

            _out.Dispose();

            _logger.LogDebug($"Written compiled file to {_outputFilePath}");
        }

        private void OutputNodes(Z80MethodCodeNode node) 
        {
            var dependencies = DependencyNodeHelpers.GetFlattenedDependencies(node);
            foreach (var dependentNode in dependencies)
            {
                if (dependentNode is EETypeNode)
                {
                    // If the type has static fields then need to reserve space for these fields

                    var typeNode = (EETypeNode)dependentNode;
                    var typeDef = typeNode.Type;
                    foreach (var field in typeDef.Fields)
                    {
                        if (field.IsStatic)
                        {
                            var fieldSize = field.FieldType.GetInstanceFieldSize();
                            _logger.LogDebug($"Reserving {fieldSize} bytes for static field {field.FullName}");

                            // Need to mangle full field name here
                            _out.WriteLine(new LabelInstruction(_nameMangler.GetMangledFieldName(field)));

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
            string[] resourceNames = GetType().Assembly.GetManifestResourceNames();
            foreach (string resourceName in resourceNames)
            {
                if (!resourceName.StartsWith("ILCompiler.Runtime")) continue;
                if (resourceName.StartsWith("ILCompiler.Runtime.TRS80") && _configuration.TargetArchitecture != TargetArchitecture.TRS80) continue;
                if (resourceName.StartsWith("ILCompiler.Runtime.CPM") && _configuration.TargetArchitecture != TargetArchitecture.CPM) continue;
                if (resourceName.StartsWith("ILCompiler.Runtime.ZXSpectrum") && _configuration.TargetArchitecture != TargetArchitecture.ZXSpectrum) continue;

                using (Stream? stream = GetType().Assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            _out.Write(reader.ReadToEnd());
                        }
                    }
                }
            }
        }
    }
}
