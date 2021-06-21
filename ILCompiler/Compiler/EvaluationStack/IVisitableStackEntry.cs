namespace ILCompiler.Compiler.EvaluationStack
{
    public interface IVisitableStackEntry
    {
        public void Accept(IStackEntryVisitor visitor);
    }
}
