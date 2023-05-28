namespace ILCompiler.Compiler.DependencyAnalysis
{
    internal class DependencyGraphTraverser
    {
        private readonly List<IDependencyNode> _visited = new List<IDependencyNode>();

        private readonly IDependencyGraphNodeVisitor _nodeVisitor;
        private readonly IDependencyGraphEdgeVisitor _edgeVisitor;

        private readonly IDependencyNode _root;

        public DependencyGraphTraverser(IDependencyGraphNodeVisitor nodeVisitor, IDependencyGraphEdgeVisitor edgeVisitor, IDependencyNode root)
        {
            _root = root;
            _nodeVisitor = nodeVisitor;
            _edgeVisitor = edgeVisitor;
        }

        public void TraverseEdges()
        {
            foreach (var depender in _visited) 
            {
                foreach (var dependedOn in depender.Dependencies)
                {
                    _edgeVisitor.VisitEdge(depender, dependedOn);
                }
            }
        }

        public void TraverseNodes()
        {
            TraverseNode(_root);
        }

        public void TraverseNode(IDependencyNode node)
        {
            _visited.Add(node);
            _nodeVisitor.VisitNode(node);

            foreach (var dependentNode in node.Dependencies)
            {
                if (!_visited.Contains(dependentNode))
                {
                    TraverseNode(dependentNode);
                }
            }
        }
    }
}
