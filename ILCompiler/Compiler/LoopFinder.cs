using ILCompiler.Compiler.Helpers;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler
{
    public class LoopFinder : ILoopFinder
    {
        public void FindLoops(IList<BasicBlock> blocks)
        {
            var reversePostOrder = DfsReversePostorderHelper.CreateDfsReversePostorder(blocks);
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