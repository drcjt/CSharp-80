using ILCompiler.Compiler.FlowgraphHelpers;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler
{
    public class LoopFinder : ILoopFinder
    {
        public void FindLoops(IList<BasicBlock> blocks)
        {
            var reversePostOrder = FlowgraphDfsTree.Build(blocks[0]).PostOrder.Reverse();
            ComputeReachabilitySets(reversePostOrder);
        }

        private static void ComputeReachabilitySets(IEnumerable<BasicBlock> reversePostOrder)
        {
            // Every block can reach itself
            foreach (var block in reversePostOrder)
            {
                block.Reach.Add(block);
            }

            foreach (var block in reversePostOrder)
            {
                foreach (var predecessor in block.Predecessors)
                {
                    // block.reachset = block.reachset union predecessor.reachset
                    block.Reach.UnionWith(predecessor.Reach);
                }
            }
        }
    }
}