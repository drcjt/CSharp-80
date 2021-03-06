using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class StringConstantCodeGenerator : ICodeGenerator<StringConstantEntry>
    {
        public void GenerateCode(StringConstantEntry entry, CodeGeneratorContext context)
        {
            // TODO: Currently obj refs can only be strings
            context.Assembler.Ld(R16.HL, entry.Label);     // LSW
            context.Assembler.Push(R16.HL);
        }
    }
}
