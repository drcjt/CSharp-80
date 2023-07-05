using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.CodeGenerators
{
    public interface ICodeGenerator<in T> where T : StackEntry
    {
        public void GenerateCode(T entry, CodeGeneratorContext context);
    }
}
