namespace ILCompiler.Compiler.Dominators
{
    public sealed class DominatorTreeNode
    {
        public BasicBlock Block { get; private set; }
        public IList<DominatorTreeNode> Children { get; set; } = new List<DominatorTreeNode>();

        public DominatorTreeNode(BasicBlock block)
        {
            Block = block;
        }
    }
}
