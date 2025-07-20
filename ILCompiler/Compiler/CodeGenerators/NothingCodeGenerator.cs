using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class NothingCodeGenerator : ICodeGenerator<NothingEntry>
    {
        public void GenerateCode(NothingEntry entry, CodeGeneratorContext context)
        {
            // No code required for NothingEntry
        }
    }
}
