using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class BinaryOrComparisionOperatorCodeGenerator : ICodeGenerator<BinaryOperator>
    {
        public void GenerateCode(BinaryOperator entry, CodeGeneratorContext context)
        {
            if (entry.IsComparison)
            {
                CompareCodeGenerator.GenerateCode(entry, context);
            }
            else
            {
                BinaryOperatorCodeGenerator.GenerateCode(entry, context);
            }
        }
    }
}
