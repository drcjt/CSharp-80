﻿using ILCompiler.Compiler.EvaluationStack;
using System;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class Int32ConstantCodeGenerator : ICodeGenerator<Int32ConstantEntry>
    {
        public void GenerateCode(Int32ConstantEntry entry, CodeGeneratorContext context)
        {
            var value = (entry as Int32ConstantEntry).Value;
            var low = BitConverter.ToInt16(BitConverter.GetBytes(value), 0);
            var high = BitConverter.ToInt16(BitConverter.GetBytes(value), 2);

            context.Assembler.Ld(R16.HL, low);
            context.Assembler.Push(R16.HL);
            context.Assembler.Ld(R16.HL, high);
            context.Assembler.Push(R16.HL);
        }
    }
}
