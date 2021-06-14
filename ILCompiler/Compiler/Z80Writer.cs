﻿using dnlib.DotNet;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.z80;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ILCompiler.Compiler
{
    public class Z80Writer
    {
        private readonly Compilation _compilation;
        private readonly StreamWriter _out;
        private readonly string _inputFilePath;
        private readonly string _outputFilePath;

        public Z80Writer(Compilation compilation, string inputFilePath, string outputFilePath)
        {
            _compilation = compilation;
            _inputFilePath = inputFilePath;
            _outputFilePath = outputFilePath;

            _out = new StreamWriter(new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, false), Encoding.ASCII);
        }

        public void CompileMethod(Z80MethodCodeNode methodCodeNodeNeedingCode)
        {
            var method = methodCodeNodeNeedingCode.Method;

            var ilImporter = new ILImporter(_compilation, method);

            ilImporter.Compile(methodCodeNodeNeedingCode);            
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

            _out.WriteLine(Instruction.Org(0x5200));

            _out.WriteLine(Instruction.Jp("START"));

            // Include the runtime assembly code
            _out.WriteLine("*include csharprt.asm");

            _out.WriteLine(new LabelInstruction("START"));

            _out.WriteLine(Instruction.Call(_compilation.NameMangler.GetMangledMethodName(entryMethod)));

            _out.WriteLine(Instruction.Pop(R16.BC));    // return value
            _out.WriteLine(Instruction.Pop(R16.HL));    // return address
            _out.WriteLine(Instruction.Push(R16.BC));
            _out.WriteLine(Instruction.Push(R16.HL));

            _out.WriteLine(Instruction.Ret());
        }

        private void OutputEpilog()
        {
            _out.WriteLine(Instruction.End("START"));
        }

        public void OutputCode(IEnumerable<Z80MethodCodeNode> nodes, MethodDef entryMethod)
        {
            OutputProlog(entryMethod);

            foreach (var node in nodes)
            {
                _out.WriteLine($"; {node.Method.FullName}");
                OutputMethodNode(node);
            }

            OutputEpilog();

            _out.Dispose();

            _compilation.Logger.LogDebug($"Written compiled file to {_outputFilePath}");
        }
    }
}
