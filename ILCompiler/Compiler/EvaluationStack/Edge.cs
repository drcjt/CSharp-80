namespace ILCompiler.Compiler.EvaluationStack
{
    public class Edge<T>(Func<T> getter, Action<T> setter)
    {
        public T Get() => getter();
        public void Set(T value) => setter(value);
    }
}
