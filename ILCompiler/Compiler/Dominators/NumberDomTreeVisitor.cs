namespace ILCompiler.Compiler.Dominators
{
    internal class NumberDomTreeVisitor : DominatorTreeVisitor
    {
        public int[] PreorderNums { get; init; }
        public int[] PostorderNums { get; init; }

        int _preorderNum = 0;
        int _postorderNum = 0;

        public NumberDomTreeVisitor(DominatorTreeNode root, int blockCount) : base(root)
        {
            PreorderNums = new int[blockCount];
            PostorderNums = new int[blockCount];
        }

        public override void PreOrderVisit(BasicBlock block)
        {
            PreorderNums[block.PostOrderNum] = _preorderNum++;
        }

        public override void PostOrderVisit(BasicBlock block)
        {
            PostorderNums[block.PostOrderNum] = _postorderNum++;
        }
    }
}
