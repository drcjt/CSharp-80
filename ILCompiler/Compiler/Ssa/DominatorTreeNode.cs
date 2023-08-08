namespace ILCompiler.Compiler.Ssa
{
    // TODO: Will need ability to do a pre order traversal of this tree
    internal sealed class DominatorTreeNode
    {
        public BasicBlock Block { get; private set; }
        public IList<DominatorTreeNode> Children { get; set; } = new List<DominatorTreeNode>();

        public DominatorTreeNode(BasicBlock block)
        {
            Block = block;
        }
    }
}
