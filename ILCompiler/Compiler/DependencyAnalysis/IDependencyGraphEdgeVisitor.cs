namespace ILCompiler.Compiler.DependencyAnalysis
{
    internal interface IDependencyGraphEdgeVisitor
    {
        void VisitEdge(IDependencyNode depender, IDependencyNode dependedOn);
    }
}
