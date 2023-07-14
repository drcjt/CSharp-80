using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class CommaCodeGenerator : ICodeGenerator<CommaEntry>
    {
        public void GenerateCode(CommaEntry entry, CodeGeneratorContext context)
        {
            // No code gen required as comma node is purely to force order of evaluation of operands
        }
    }
}
