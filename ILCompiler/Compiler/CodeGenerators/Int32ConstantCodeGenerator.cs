using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Common.TypeSystem.IL;
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

            if (entry.Kind == StackValueKind.NativeInt)
            {
                // Native ints are only 16 bit so just push low word
                context.Assembler.Ld(R16.HL, low);
                context.Assembler.Push(R16.HL);
            }
            else
            {
                context.Assembler.Ld(R16.HL, high);     //MSW
                context.Assembler.Push(R16.HL);
                context.Assembler.Ld(R16.HL, low);      //LSW
                context.Assembler.Push(R16.HL);
            }
        }
    }
}