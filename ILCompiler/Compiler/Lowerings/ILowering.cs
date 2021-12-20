using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.Lowerings
{
    public interface ILowering<T> where T : StackEntry
    {
        public StackEntry? Lower(T entry);
    }
}
