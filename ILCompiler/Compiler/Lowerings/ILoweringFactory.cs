using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.Lowerings
{
    public interface ILoweringFactory
    {
        ILowering<T>? GetLowering<T>() where T : StackEntry;
    }
}
