using System.Text;
using ILCompiler.Compiler.FlowgraphHelpers;
using ILCompiler.Interfaces;
using Microsoft.Extensions.Logging;

namespace ILCompiler.Compiler.Dominators
{
    internal class FlowgraphDominatorTreeBuilder : IComputeDominators
    {
        private readonly ILogger<FlowgraphDominatorTreeBuilder> _logger;

        public FlowgraphDominatorTreeBuilder(ILogger<FlowgraphDominatorTreeBuilder> logger)
        {
            _logger = logger;
        }


        public FlowgraphDominatorTree Build(FlowgraphDfsTree dfs, IList<BasicBlock> blocks)
        {
            // Topologically sort the graph
            var postOrder = dfs.PostOrder;

            // Compute the immediate dominators of all basic blocks
            ComputeImmediateDominators(postOrder);

            // Create the dominator tree
            var root = BuildDominatorTree(blocks);

            // Assign preorder and postorder numbers for quick dominance checks
            NumberDomTreeVisitor numberDomTreeVisitor = new NumberDomTreeVisitor(root, postOrder.Count);
            numberDomTreeVisitor.WalkTree();

            return new FlowgraphDominatorTree(dfs, root, numberDomTreeVisitor.PreorderNums, numberDomTreeVisitor.PostorderNums);
        }

        private DominatorTreeNode BuildDominatorTree(IList<BasicBlock> blocks)
        {
            _logger.LogDebug("[FlowgraphDominatorBuilder:BuildDominatorTree])");

            var nodeMap = new Dictionary<BasicBlock, DominatorTreeNode>();

            DominatorTreeNode? rootNode = null;

            foreach (var block in blocks)
            {
                var immediateDominator = block.ImmediateDominator;
                if (immediateDominator != null)
                {
                    DominatorTreeNode node = GetOrCreate(immediateDominator, nodeMap);
                    DominatorTreeNode childNode = GetOrCreate(block, nodeMap);
                    nodeMap[block] = childNode;
                    node.Children.Add(childNode);
                }
                else
                {
                    rootNode = new DominatorTreeNode(block);
                    nodeMap.Add(block, rootNode);
                }
            }

            foreach (var node in nodeMap)
            {
                var sb = new StringBuilder($"{node.Key.Label} : ");
                foreach (var childNode in node.Value.Children)
                {
                    sb.Append($"{childNode.Block.Label} ");
                }
                _logger.LogDebug(sb.ToString());
            }

            return rootNode!;
        }

        private static DominatorTreeNode GetOrCreate(BasicBlock block, Dictionary<BasicBlock, DominatorTreeNode> nodeMap)
        {
            if (!nodeMap.TryGetValue(block, out var node))
            {
                node = new DominatorTreeNode(block);
                nodeMap[block] = node;
            }
            return node;
        }

        private void ComputeImmediateDominators(IList<BasicBlock> postOrder)
        {
            _logger.LogDebug("[FlowgraphDominatorBuilder:ComputeImmediateDominators])");

            var count = postOrder.Count;

            var processedBlocks = new HashSet<BasicBlock>
            {
                postOrder[postOrder.Count - 1]
            };

            bool changed = true;
            while (changed)
            {
                changed = false;

                // In reverse post order except for the entry block, (count - 1) is the entry block
                for (int i = count - 2; i >= 0; --i)
                {
                    var block = postOrder[i];

                    _logger.LogDebug("Visiting in reverse post order: {BlockLabel}", block.Label);

                    // Find the first processed predecessor
                    BasicBlock? predecessorBlock = null;
                    foreach (var predecessor in BlockDominancePredecessors(block))
                    {
                        if (processedBlocks.Contains(predecessor))
                        {
                            predecessorBlock = predecessor;
                            break;
                        }
                    }

                    if (predecessorBlock != null)
                    {
                        _logger.LogDebug("Predecessor block is {PredecessorBlockLabel}", predecessorBlock.Label);
                    }

                    // Intersect DOM, if computed for all predecessors
                    BasicBlock? basicBlockDominator = null;
                    foreach (var predecessor in BlockDominancePredecessors(block))
                    {
                        var domPred = predecessor;

                        // Skip predecessors not yet visited
                        if (domPred.PostOrderNum <= i)
                            continue;

                        if (basicBlockDominator is null)
                        {
                            basicBlockDominator = domPred;
                        }
                        else
                        {
                            basicBlockDominator = FlowgraphDominatorTree.Intersects(basicBlockDominator, domPred);
                        }
                    }

                    // Did we change the immediate dominator?
                    // if so then go around the outer loop again
                    if (block.ImmediateDominator != basicBlockDominator)
                    {
                        changed = true;

                        _logger.LogDebug("ImmediateDominator of {BlockLabel} becomes {BasicBlockDominatorLabel}", block.Label, basicBlockDominator?.Label);
                        block.ImmediateDominator = basicBlockDominator;
                    }

                    _logger.LogDebug("Marking block {BlockLabel} as processed", block.Label);
                    processedBlocks.Add(block);
                }
            }
        }

        /// <summary>
        /// Calculates the predecessor blocks that have fully executed before block was reached.
        /// Only differs for handler blocks as the try blocks may not have fully executed 
        /// so we use the first block in the try as the predecessor 
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <returns>list of dominance predecessors</returns>
        private static IList<BasicBlock> BlockDominancePredecessors(BasicBlock block)
        {
            if (!block.HandlerStart)
            {
                return block.Predecessors;
            }
            else
            {
                return new List<BasicBlock> { block.TryBlocks[0] };
            }
        }
    }
}
