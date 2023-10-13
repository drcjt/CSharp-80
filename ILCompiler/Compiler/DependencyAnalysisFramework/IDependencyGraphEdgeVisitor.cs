namespace ILCompiler.Compiler.DependencyAnalysisFramework
{
    internal interface IDependencyGraphEdgeVisitor
    {
        void VisitEdge(IDependencyNode depender, IDependencyNode dependedOn);
    }
}
