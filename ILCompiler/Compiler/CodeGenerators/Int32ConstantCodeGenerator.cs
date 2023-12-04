using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class Int32ConstantCodeGenerator : ICodeGenerator<Int32ConstantEntry>
    {
        public void GenerateCode(Int32ConstantEntry entry, CodeGeneratorContext context)
        {
            var value = entry.Value;
            var low = BitConverter.ToInt16(BitConverter.GetBytes(value), 0);
            var high = BitConverter.ToInt16(BitConverter.GetBytes(value), 2);

            context.InstructionsBuilder.Ld(DE, high);     //MSW
            context.InstructionsBuilder.Ld(HL, low);      //LSW
            context.InstructionsBuilder.Push(DE);
            context.InstructionsBuilder.Push(HL);
        }
    }
}