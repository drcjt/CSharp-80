using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.CodeGenerators
{
    public interface ICodeGeneratorFactory
    {
        ICodeGenerator<T> GetCodeGenerator<T>() where T : StackEntry;
    }
}
