using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class Int32ConstantCodeGenerator : ICodeGenerator<Int32ConstantEntry>
    {
        public void GenerateCode(Int32ConstantEntry entry, CodeGeneratorContext context)
        {
            var value = (entry as Int32ConstantEntry).Value;
            var low = BitConverter.ToInt16(BitConverter.GetBytes(value), 0);
            var high = BitConverter.ToInt16(BitConverter.GetBytes(value), 2);

            context.Emitter.Ld(R16.HL, high);     //MSW
            context.Emitter.Push(R16.HL);
            context.Emitter.Ld(R16.HL, low);      //LSW
            context.Emitter.Push(R16.HL);
        }
    }
}