using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.PreInit;
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
        private readonly PreinitializationManager _preinitializationManager;
        private readonly NodeFactory _nodeFactory;

        private string _inputFilePath = null!;
        private string _outputFilePath = null!;
        private StreamWriter _out = null!;

        private readonly ISet<string> _calls = new HashSet<string>();

        public Z80Writer(IConfiguration configuration, INameMangler nameMangler, ILogger<Z80Writer> logger, NativeDependencyAnalyser nativeDependencyAnalyser, PreinitializationManager preinitializzationManager, NodeFactory nodeFactory)
        {
            _configuration = configuration;
            _nameMangler = nameMangler;
            _logger = logger;
            _nativeDependencyAnalyser = nativeDependencyAnalyser;
            _preinitializationManager = preinitializzationManager;
            _nodeFactory = nodeFactory;
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

        private void OutputCodeForNodes(IList<IDependencyNode> nodes)
        {
            foreach (var node in nodes)
            {
                if (node is Z80MethodCodeNode codeNode && codeNode.MethodCode != null)
                {
                    _out.WriteLine($"; {codeNode.Method.FullName}");
                    OutputMethodNode(codeNode);
                }
            }
        }

        public void OutputCode(Z80MethodCodeNode rootNode, IList<IDependencyNode> nodes, string inputFilePath, string outputFilePath)
        {
            _inputFilePath = inputFilePath;
            _outputFilePath = outputFilePath;

            using (_out = new StreamWriter(new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, false), Encoding.ASCII))
            {
                OutputProlog(rootNode);

                OutputStatics(nodes);
                OutputEETypes(nodes);
                OutputCodeForNodes(nodes);

                OutputEpilog();
            }

            _logger.LogDebug("Written compiled file to {_outputFilePath}", _outputFilePath);
        }

        private void OutputStatics(IList<IDependencyNode> nodes)
        {
            foreach (var node in nodes)
            {
                // For static field dependencies we need to reserve appropriate space for the field
                if (node is StaticsNode typeNode)
                {
                    var field = typeNode.Field;

                    if (_preinitializationManager.IsPreinitialized(field.DeclaringType))
                    {
                        var preinitializationInfo = _preinitializationManager.GetPreinitializationInfo(field.DeclaringType);
                        var value = preinitializationInfo.GetFieldValue(field);

                        // Need to mangle full field name here
                        OutputLabel(_nameMangler.GetMangledFieldName(field));

                        var bytes = value.GetRawData();
                        foreach (var b in bytes)
                        {
                            _out.WriteLine(Instruction.Create(Opcode.Db, b));
                        }
                    }
                    else
                    {
                        var fieldSize = field.FieldType.GetInstanceFieldSize();
                        _logger.LogDebug("Reserving {fieldSize} bytes for static field {field.FullName}", fieldSize, field.FullName);

                        // Need to mangle full field name here
                        OutputLabel(_nameMangler.GetMangledFieldName(field));

                        // Emit fieldSize bytes with value 0
                        _out.WriteLine(Instruction.Create(Opcode.Dc, (ushort)fieldSize, 0));
                    }
                }
            }
        }

        private void OutputEETypes(IList<IDependencyNode> nodes)
        {
            var eeTypes = new List<string>();

            foreach (var node in nodes)
            {
                // For static field dependencies we need to reserve appropriate space for the field
                if (node is ConstructedEETypeNode typeNode)
                {
                    var eeMangledTypeName = _nameMangler.GetMangledTypeName(typeNode.Type);

                    if (!eeTypes.Contains(eeMangledTypeName))
                    {
                        eeTypes.Add(eeMangledTypeName);

                        _out.WriteLine($";{typeNode.Type.FullName}");
                        // Need to mangle full field name here
                        OutputLabel(eeMangledTypeName);

                        // Emit data for EEType flags here
                        ushort flags = 0;
                        if (_preinitializationManager.HasLazyStaticConstructor(typeNode.Type))
                        {
                            flags = 1;
                        }
                        _out.WriteLine(Instruction.Create(Opcode.Dw, flags));

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

                        // Emit VTable
                        OutputVirtualSlots(typeNode.Type, typeNode.Type);
                    }
                }
            }
        }

        private void OutputVirtualSlots(ITypeDefOrRef type, ITypeDefOrRef implType)
        {
            // Output inherited VTable slots first
            var baseType = type.GetBaseType();
            if (baseType != null)
            {
                OutputVirtualSlots(baseType, implType);
            }

            // Now get new slots
            var resolvedType = type.ResolveTypeDefThrow();
            var vTable = _nodeFactory.VTable(resolvedType);
            var virtualSlots = vTable.GetSlots();

            // Emit VTable entries for the new slots
            for (int i = 0; i < virtualSlots.Count; i++)
            {
                var method = virtualSlots[i];
                var implementation = VirtualMethodAlgorithm.FindVirtualFunctionTargetMethodOnObjectType(implType.ResolveTypeDefThrow(), method);

                // Only generate slot entries for non abstract methods
                if (implementation != null && !implementation.IsAbstract)
                {
                    var implementationMangledName = _nameMangler.GetMangledMethodName(implementation);
                    _out.WriteLine(Instruction.Create(Opcode.Dw, implementationMangledName));
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
