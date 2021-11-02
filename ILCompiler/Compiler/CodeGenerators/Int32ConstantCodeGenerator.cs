using ILCompiler.Compiler.EvaluationStack;
using System;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    public class Int32ConstantCodeGenerator
    {
        public static void GenerateCode(Int32ConstantEntry entry, Assembler assembler)
        {
            var value = (entry as Int32ConstantEntry).Value;
            var low = BitConverter.ToInt16(BitConverter.GetBytes(value), 0);
            var high = BitConverter.ToInt16(BitConverter.GetBytes(value), 2);

            assembler.Ld(R16.HL, low);
            assembler.Push(R16.HL);
            assembler.Ld(R16.HL, high);
            assembler.Push(R16.HL);
        }

    }
}
