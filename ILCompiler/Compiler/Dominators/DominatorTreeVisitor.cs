namespace ILCompiler.Compiler.Dominators
{
    internal class DominatorTreeVisitor
    {
        readonly DominatorTreeNode _root;
        public DominatorTreeVisitor(DominatorTreeNode root)
        {
            _root = root;
        }

        public virtual void PreOrderVisit(BasicBlock block)
        {

        }

        public virtual void PostOrderVisit(BasicBlock block)
        {
        }

        public void WalkTree() => WalkTree(_root);

        private void WalkTree(DominatorTreeNode node)
        {
            PreOrderVisit(node.Block);

            foreach (var child in node.Children)
            {
                WalkTree(child);
            }

            PostOrderVisit(node.Block);
        }
    }
}
