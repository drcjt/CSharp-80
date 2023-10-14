using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class NativeIntConstantCodeGenerator : ICodeGenerator<NativeIntConstantEntry>
    {
        public void GenerateCode(NativeIntConstantEntry entry, CodeGeneratorContext context)
        {
            if (entry.SymbolName != String.Empty)
            {
                context.InstructionsBuilder.Ld(HL, entry.SymbolName);
            }
            else
            {
                var value = entry.Value;
                var low = BitConverter.ToInt16(BitConverter.GetBytes(value), 0);

                // Native ints are only 16 bit so just push low word
                context.InstructionsBuilder.Ld(HL, low);
            }

            context.InstructionsBuilder.Push(HL);
        }
    }
}