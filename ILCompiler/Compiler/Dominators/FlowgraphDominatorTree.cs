using ILCompiler.Compiler.FlowgraphHelpers;

namespace ILCompiler.Compiler.Dominators
{
    public class FlowgraphDominatorTree
    {
        public FlowgraphDfsTree DfsTree { get; init; }
        public DominatorTreeNode Root { get; init; }
        private readonly int[] _preorderNums;
        private readonly int[] _postorderNums;

        public FlowgraphDominatorTree(FlowgraphDfsTree dfsTree, DominatorTreeNode dominatorTree, int[] preorderNums, int[] postorderNums)
        {
            DfsTree = dfsTree;
            Root = dominatorTree;
            _preorderNums = preorderNums;
            _postorderNums = postorderNums;
        }

        public static BasicBlock? Intersects(BasicBlock block, BasicBlock block2) => IntersectDominators(block, block2);

        public bool Dominates(BasicBlock dominator, BasicBlock dominated)
        {
            return _preorderNums[dominator.PostOrderNum] <= _preorderNums[dominated.PostOrderNum] &&
                   _postorderNums[dominator.PostOrderNum] >= _postorderNums[dominated.PostOrderNum];
        }

        private static BasicBlock? IntersectDominators(BasicBlock? finger1, BasicBlock? finger2)
        {
            while (finger1 != finger2)
            {
                if (finger1 == null || finger2 == null)
                {
                    return null;
                }

                while (finger1 != null && finger1.PostOrderNum < finger2.PostOrderNum)
                {
                    finger1 = finger1.ImmediateDominator;
                }
                if (finger1 == null)
                {
                    return null;
                }

                while (finger2 != null && finger2.PostOrderNum < finger1.PostOrderNum)
                {
                    finger2 = finger2.ImmediateDominator;
                }
            }
            return finger1;
        }
    }
}
