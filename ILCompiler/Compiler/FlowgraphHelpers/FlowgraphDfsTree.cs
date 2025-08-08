namespace ILCompiler.Compiler.FlowgraphHelpers
{
    public class FlowgraphDfsTree(IList<BasicBlock> postOrder, bool hasCycle)
    {
        public IList<BasicBlock> PostOrder { get; init; } = postOrder;
        public bool HasCycle { get; init; } = hasCycle;

        public class AllSuccessorEnumerator(BasicBlock block)
        {
            private int _position = 0;
            public BasicBlock Block { get; init; } = block;

            public BasicBlock? NextSuccessor => _position < Block.Successors.Count ? Block.Successors[_position++] : null;
        }

        public static bool IsAncestor(BasicBlock ancestor, BasicBlock descendant)
        {
            return ancestor.PreOrderNum <= descendant.PreOrderNum &&
                descendant.PostOrderNum <= ancestor.PostOrderNum;
        }

        public static FlowgraphDfsTree BuildAndRemove(IList<BasicBlock> blocks)
        {
            blocks = SetupBasicBlockRoot(blocks);

            var firstBlock = blocks[0];
            var dfsTree = Build(firstBlock);

            dfsTree.RemoveBlocksOutsideOfDfsTree(blocks);

            return dfsTree;
        }

        private static IList<BasicBlock> SetupBasicBlockRoot(IList<BasicBlock> blocks)
        {
            if (blocks[0].Predecessors.Count != 0)
            {
                // Need to create a new basic block to act as the loop as the real first block is a loop
                var basicBlockRoot = new BasicBlock(0);
                basicBlockRoot.Successors.Add(blocks[0]);
                blocks[0].Predecessors.Add(basicBlockRoot);

                blocks.Insert(0, basicBlockRoot);
            }

            return blocks;
        }

        private void RemoveBlocksOutsideOfDfsTree(IList<BasicBlock> blocks)
        {
            var blocksToRemove = blocks.Where(block => !PostOrder.Contains(block) && !block.HandlerStart).ToList();
            foreach (var block in blocksToRemove)
            {
                blocks.Remove(block);
            }
        }

        public static FlowgraphDfsTree Build(BasicBlock firstBlock)
        {
            var postOrder = new List<BasicBlock>();
            bool hasCycle = false;

            var visitedBlocks = new HashSet<BasicBlock>();

            uint postOrderIndex = 0;
            uint preOrderIndex = 0;

            var blocks = new Stack<AllSuccessorEnumerator>();
            blocks.Push(new AllSuccessorEnumerator(firstBlock));
            VisitPreOrder(firstBlock, preOrderIndex++);

            while (blocks.Count != 0)
            {
                var block = blocks.Peek().Block;
                var successor = blocks.Peek().NextSuccessor;

                if (successor != null)
                {
                    if (!visitedBlocks.Contains(successor))
                    {
                        blocks.Push(new AllSuccessorEnumerator(successor));
                        visitedBlocks.Add(successor);

                        VisitPreOrder(successor, preOrderIndex++);
                    }

                    VisitEdge(block, successor, ref hasCycle);
                }
                else
                {
                    blocks.Pop();
                    VisitPostOrder(block, postOrderIndex++, postOrder);
                }
            }

            return new FlowgraphDfsTree(postOrder, hasCycle);
        }

        private static void VisitPreOrder(BasicBlock block, uint preOrderIndex)
        {
            block.PreOrderNum = preOrderIndex;
            block.PostOrderNum = uint.MaxValue;
        }

        private static void VisitPostOrder(BasicBlock block, uint postOrderIndex, List<BasicBlock> postOrder)
        {
            block.PostOrderNum = postOrderIndex;
            postOrder.Add(block);
        }

        private static void VisitEdge(BasicBlock block, BasicBlock successor, ref bool hasCycle)
        {
            // Check if block -> successor is a back edge, if so then the graph has a cycle
            if ((successor.PreOrderNum <= block.PreOrderNum) && successor.PostOrderNum == uint.MaxValue)
            {
                hasCycle = true;
            }
        }
    }
}
