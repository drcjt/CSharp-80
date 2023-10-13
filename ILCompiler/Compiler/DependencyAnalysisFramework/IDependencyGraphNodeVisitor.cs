namespace ILCompiler.Compiler.DependencyAnalysisFramework
{
    internal interface IDependencyGraphNodeVisitor
    {
        public void VisitNode(IDependencyNode node);
    }
}
