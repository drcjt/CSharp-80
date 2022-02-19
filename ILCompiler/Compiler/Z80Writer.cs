using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
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
                foreach (var instruction in methodCodeNode.MethodCode)
                {
                    _out.WriteLine(instruction.ToString());
                }
            }
        }

        private short GetOrgAddress()
        {
            if (_configuration.IntegrationTests)
            {
                return 0x0000;
            }
            if (_configuration.TargetCpm)
            {
                return 0x100;
            }
            return 0x5200;
        }

        private void OutputProlog(MethodDef entryMethod)
        {
            _out.WriteLine($"; INPUT FILE {_inputFilePath.ToUpper()}");
            _out.WriteLine($"; {DateTime.Now}");
            _out.WriteLine();

            var org = GetOrgAddress();

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

            var hasReturnCode = entryMethod.ReturnType.GetStackValueKind() == StackValueKind.Int32;

            if (hasReturnCode && _configuration.PrintReturnCode)
            {
                _out.WriteLine(Instruction.Db("Return Code:", "retcodemsg"));
                _out.WriteLine(Instruction.Db(0));
            }

            // Include the runtime assembly code
            if (_configuration.DontInlineRuntime)
            {
                _out.WriteLine("include csharprt.asm");
                if (_configuration.TargetCpm)
                {
                    _out.WriteLine("include cpmrt.asm");
                }
                else
                {
                    _out.WriteLine("include trs80rt.asm");
                }
            }

            _out.WriteLine(new LabelInstruction("START"));

            _out.WriteLine(Instruction.Call(_nameMangler.GetMangledMethodName(entryMethod)));

            if (hasReturnCode && _configuration.PrintReturnCode)
            {
                // Write string "Return Code:"
                _out.WriteLine(Instruction.Ld(R16.HL, "retcodemsg"));
                _out.WriteLine(Instruction.Call("PRINT"));

                _out.WriteLine(Instruction.Pop(R16.DE));
                _out.WriteLine(Instruction.Pop(R16.HL));
                _out.WriteLine(Instruction.Push(R16.HL));
                _out.WriteLine(Instruction.Push(R16.DE));
                _out.WriteLine(Instruction.Call("LTOA"));
            }

            if (hasReturnCode)
            {
                _out.WriteLine(Instruction.Pop(R16.BC));    // return value
                _out.WriteLine(Instruction.Pop(R16.DE));
                _out.WriteLine(Instruction.Pop(R16.HL));    // return address
                _out.WriteLine(Instruction.Push(R16.DE));
                _out.WriteLine(Instruction.Push(R16.BC));
                _out.WriteLine(Instruction.Push(R16.HL));
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
                if (node.MethodCode != null)
                {
                    _out.WriteLine($"; {node.Method.FullName}");

                    OutputMethodNode(node);
                }

                if (node.Dependencies != null)
                {
                    foreach (var dependentNodes in node.Dependencies)
                    {
                        OutputCodeForNode(dependentNodes);
                    }
                }
                node.CodeEmitted = true;
            }
        }

        public void OutputCode(Z80MethodCodeNode root, string inputFilePath, string outputFilePath)
        {
            _inputFilePath = inputFilePath;
            _outputFilePath = outputFilePath;
            _out = new StreamWriter(new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, false), Encoding.ASCII);

            OutputProlog(root.Method);

            OutputCodeForNode(root);

            OutputEpilog();

            _out.Dispose();

            _logger.LogDebug($"Written compiled file to {_outputFilePath}");
        }

        private void OutputRuntimeCode()
        {
            _out.WriteLine();
            _out.WriteLine("; **** Runtime starts here");
            string[] resourceNames = GetType().Assembly.GetManifestResourceNames();
            foreach (string resourceName in resourceNames)
            {
                if (resourceName.StartsWith("ILCompiler.Runtime.TRS80") && _configuration.TargetCpm) continue;
                if (resourceName.StartsWith("ILCompiler.Runtime.CPM") && !_configuration.TargetCpm) continue;

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
