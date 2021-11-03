using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal interface ICodeGenerator<T> where T : StackEntry
    {
        public void GenerateCode(T entry, CodeGeneratorContext context);
    }
}
