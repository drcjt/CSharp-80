using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class StringConstantCodeGenerator : ICodeGenerator<StringConstantEntry>
    {
        public void GenerateCode(StringConstantEntry entry, CodeGeneratorContext context)
        {
            // TODO: Currently obj refs can only be strings
            context.Emitter.Ld(HL, entry.Label);     // LSW
            context.Emitter.Push(HL);
        }
    }
}
