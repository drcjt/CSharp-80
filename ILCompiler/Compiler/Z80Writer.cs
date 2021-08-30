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
        private readonly Compilation _compilation;
        private readonly StreamWriter _out;
        private readonly string _inputFilePath;
        private readonly string _outputFilePath;
        private readonly IConfiguration _configuration;

        public Z80Writer(Compilation compilation, string inputFilePath, string outputFilePath, IConfiguration configuration)
        {
            _compilation = compilation;
            _inputFilePath = inputFilePath;
            _outputFilePath = outputFilePath;
            _configuration = configuration;

            _out = new StreamWriter(new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, false), Encoding.ASCII);
        }

        private void OutputMethodNode(Z80MethodCodeNode methodCodeNode)
        {
            foreach (var instruction in methodCodeNode.MethodCode)
            {
                _out.WriteLine(instruction.ToString());
            }
        }

        private void OutputProlog(MethodDef entryMethod)
        {
            _out.WriteLine($"; INPUT FILE {_inputFilePath.ToUpper()}");
            _out.WriteLine($"; {DateTime.Now}");
            _out.WriteLine();

            if (!_configuration.IntegrationTests)
            {
                _out.WriteLine(Instruction.Org(0x5200));
                _out.WriteLine(Instruction.Jp("START"));
            }
            else
            {
                _out.WriteLine(Instruction.Org(0x0000));
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
            }

            _out.WriteLine(new LabelInstruction("START"));

            _out.WriteLine(Instruction.Call(_compilation.NameMangler.GetMangledMethodName(entryMethod)));

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

            _out.WriteLine(Instruction.End("START"));
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

                foreach (var dependentNodes in node.Dependencies)
                {
                    OutputCodeForNode(dependentNodes);
                }
                node.CodeEmitted = true;
            }
        }

        public void OutputCode(Z80MethodCodeNode root)
        {
            OutputProlog(root.Method);

            OutputCodeForNode(root);

            OutputEpilog();

            _out.Dispose();

            _compilation.Logger.LogDebug($"Written compiled file to {_outputFilePath}");
        }

        private void OutputRuntimeCode()
        {
            _out.WriteLine();
            _out.WriteLine("; **** Runtime starts here");
            string[] resourceNames = GetType().Assembly.GetManifestResourceNames();
            foreach (string resourceName in resourceNames)
            {
                using (Stream stream = GetType().Assembly.GetManifestResourceStream(resourceName))
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
