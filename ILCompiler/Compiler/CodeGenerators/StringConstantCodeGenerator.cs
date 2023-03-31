using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class StringConstantCodeGenerator : ICodeGenerator<StringConstantEntry>
    {
        public void GenerateCode(StringConstantEntry entry, CodeGeneratorContext context)
        {
            // TODO: Currently obj refs can only be strings
            context.Emitter.Ld(R16.HL, entry.Label);     // LSW
            context.Emitter.Push(R16.HL);
        }
    }
}
