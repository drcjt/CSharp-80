namespace ILCompiler.Compiler.DependencyAnalysis
{
    internal interface IDependencyGraphNodeVisitor
    {
        public void VisitNode(IDependencyNode node);
    }
}
