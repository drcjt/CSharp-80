using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal interface ICodeGeneratorFactory
    {
        ICodeGenerator<T> GetCodeGenerator<T>() where T : StackEntry;
    }
}
