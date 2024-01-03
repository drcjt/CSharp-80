using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class CatchArgumentCodeGenerator : ICodeGenerator<CatchArgumentEntry>
    {
        public void GenerateCode(CatchArgumentEntry entry, CodeGeneratorContext context)
        {
            // No codegen required
        }
    }
}
