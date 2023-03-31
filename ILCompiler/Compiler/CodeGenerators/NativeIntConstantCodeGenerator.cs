using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class NativeIntConstantCodeGenerator : ICodeGenerator<NativeIntConstantEntry>
    {
        public void GenerateCode(NativeIntConstantEntry entry, CodeGeneratorContext context)
        {
            var value = (entry as NativeIntConstantEntry).Value;
            var low = BitConverter.ToInt16(BitConverter.GetBytes(value), 0);

            // Native ints are only 16 bit so just push low word
            context.Emitter.Ld(R16.HL, low);
            context.Emitter.Push(R16.HL);
        }
    }
}