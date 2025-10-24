using System.Diagnostics;
using ILCompiler.Compiler.FlowgraphHelpers;
using ILCompiler.Interfaces;
using Microsoft.Extensions.Logging;

namespace ILCompiler.Compiler
{
    public class LoopFinder : ILoopFinder
    {
        private readonly ILogger<LoopFinder> _logger;
        public LoopFinder(ILogger<LoopFinder> logger) 
        {
            _logger = logger;
        }

        public FlowGraphNaturalLoops FindLoops(IList<BasicBlock> blocks, FlowgraphDfsTree dfs)
        {
            var loops = FlowGraphNaturalLoops.Find(dfs, _logger);

            // Consider moving out of here
            ComputeReachabilitySets(dfs);

            return loops;
        }

        private static void ComputeReachabilitySets(FlowgraphDfsTree dfs)
        {
            IEnumerable<BasicBlock> reversePostOrder = dfs.PostOrder.Reverse();

            // Every block can reach itself
            foreach (BasicBlock? block in reversePostOrder)
            {
                block.Reach.Add(block);
            }

            foreach (BasicBlock? block in reversePostOrder)
            {
                foreach (BasicBlock predecessor in block.Predecessors)
                {
                    // block.reachset = block.reachset union predecessor.reachset
                    block.Reach.UnionWith(predecessor.Reach);
                }
            }
        }
    }

    public class FlowGraphNaturalLoop(FlowgraphDfsTree dfsTree, BasicBlock header)
    {
        public FlowgraphDfsTree DfsTree { get; init; } = dfsTree;
        public BasicBlock Header { get; } = header;
        public IList<FlowEdge> BackEdges { get; } = [];
        public IList<FlowEdge> EntryEdges { get; } = [];
        public IList<FlowEdge> ExitEdges { get; } = [];

        public int Index { get; set; }

        public FlowGraphNaturalLoop? Parent { get; set; }
        public FlowGraphNaturalLoop? Child { get; set; }
        public FlowGraphNaturalLoop? Sibling { get; set; }

        public IList<BasicBlock> Blocks { get; } = [];

        public bool ContainsImproperHeader { get; set; } = false;

        public bool VisitLoopBlocksReversePostOrder(Func<BasicBlock, bool> visitLoopBlock)
        {
            foreach (BasicBlock? block in Blocks.OrderByDescending(x => x.PostOrderNum))
            {
                if (!visitLoopBlock(block))
                    return false;
            }
            return true;
        }

        public bool ContainsBlock(BasicBlock block)
        {
            if (!DfsTree.PostOrder.Contains(block))
                return false;

            return Blocks.Contains(block);
        }

        public bool MayExecuteBlockMultipleTimesPerIteration(BasicBlock block)
        {
            Debug.Assert(ContainsBlock(block));

            if (ContainsImproperHeader)
            {
                return true;
            }

            for (FlowGraphNaturalLoop? child = Child; child is not null; child = child.Sibling)
            {
                if (child.ContainsBlock(block))
                    return true;

            }

            return false;
        }

        public bool IsPostDominatedOnLoopIteration(BasicBlock block, BasicBlock postDominator)
        {
            Stack<BasicBlock> stack = [];

            stack.Push(block);

            HashSet<BasicBlock> visited = [];

            bool result = true;
            while (stack.Count > 0)
            {
                BasicBlock topBlock = stack.Pop();
                if (topBlock == postDominator)
                {
                    continue;
                }

                foreach (BasicBlock succ in topBlock.Successors)
                {
                    if (succ == Header)
                    {
                        result = false;
                        break;
                    }

                    if (!ContainsBlock(succ))
                    {
                        // Block is not inside loop
                        continue;
                    }

                    if (visited.Add(succ))
                    {
                        stack.Push(succ);
                    }
                }
            }

            return result;
        }
    }

    public class FlowGraphNaturalLoops()
    {
        private readonly IList<FlowGraphNaturalLoop> _loops = [];

        public IList<FlowGraphNaturalLoop> Loops => _loops;
        public IEnumerable<FlowGraphNaturalLoop> InPostOrder => _loops.Reverse();

        public void Add(FlowGraphNaturalLoop loop)
        {
            _loops.Add(loop);
        }

        public static FlowGraphNaturalLoops Find(FlowgraphDfsTree dfsTree, ILogger<LoopFinder> logger)
        {
            logger.LogDebug("Finding loops in DFS tree");
            logger.LogDebug("Reverse Post Order -> Block [pre, post]");

            for (int i = dfsTree.PostOrder.Count; i != 0; i--)
            {
                int rpoNumber = dfsTree.PostOrder.Count - i;
                BasicBlock block = dfsTree.PostOrder[i-1];
                logger.LogDebug("{RpoNumber} -> {Label} [{PreOrderNum}, {PostOrderNum}]", rpoNumber, block.Label, block.PreOrderNum, block.PostOrderNum);
            }

            var loops = new FlowGraphNaturalLoops();

            // Check if tree has any cycles if not skip
            if (!dfsTree.HasCycle)
            {
                logger.LogDebug("Flow graph has no cycles. Skipping identification of natural loops");

                return loops;
            }

            foreach (BasicBlock? header in dfsTree.PostOrder.Reverse())
            {
                FlowGraphNaturalLoop? loop = null;

                // If a block is a ancestor of one of its predecessors then the block is a loop header
                loop = FindBackEdges(dfsTree, logger, header, loop);

                if (loop == null)
                    continue;

                logger.LogDebug("{Label} is the header of a DFS loop with {Count} backedges", header.Label, loop.BackEdges.Count);

                // Walk backwards in flow along the back edges from header to determine if this 
                // is a natural loop and to find all of the basic blocks in the loop
                var worklist = new Stack<BasicBlock>();
                if (!FindNaturalLoopBlocks(loop, worklist, logger))
                {
                    foreach (FlowGraphNaturalLoop otherLoop in loops.InPostOrder)
                    {
                        if (otherLoop.ContainsBlock(header))
                        {
                            // Improper loop
                            logger.LogDebug("Loop contains an improper loop header");
                            otherLoop.ContainsImproperHeader = true;
                        }
                    }

                    continue;
                }

                logger.LogDebug("Loop has {Count} blocks", loop.Blocks.Count);

                // Find the exit edges
                FindExitEdges(logger, loop);

                // Find the entry edges
                FindEntryEdges(dfsTree, logger, header, loop);

                // Search for parent loop
                FindParentLoop(logger, loops, header, loop);

                loop.Index = loops.Loops.Count;
                loops.Add(loop);

                logger.LogDebug("Added loop with header {Label}", loop.Header.Label);
            }

            // Setup the sibling & child links by iterating the loops in post order.
            // This ends up with the sibling links in reverse post order
            foreach (FlowGraphNaturalLoop loop in loops.InPostOrder)
            {
                if (loop.Parent is not null)
                {
                    loop.Sibling = loop.Parent.Child;
                    loop.Parent.Child = loop;
                }
            }

            return loops;
        }

        private static FlowGraphNaturalLoop? FindBackEdges(FlowgraphDfsTree dfsTree, ILogger<LoopFinder> logger, BasicBlock header, FlowGraphNaturalLoop? loop)
        {
            foreach (BasicBlock predecessorBlock in header.Predecessors)
            {
                if (dfsTree.PostOrder.Contains(predecessorBlock) && FlowgraphDfsTree.IsAncestor(header, predecessorBlock))
                {
                    loop = loop ?? new FlowGraphNaturalLoop(dfsTree, header);
                    loop.BackEdges.Add(new FlowEdge(predecessorBlock, header));

                    logger.LogDebug("{PredecssorLabel} -> {HeaderLabel} is a backedge", predecessorBlock.Label, header.Label);
                }
            }

            return loop;
        }

        private static void FindParentLoop(ILogger<LoopFinder> logger, FlowGraphNaturalLoops loops, BasicBlock header, FlowGraphNaturalLoop loop)
        {
            foreach (FlowGraphNaturalLoop otherLoop in loops.InPostOrder)
            {
                if (otherLoop.ContainsBlock(header))
                {
                    loop.Parent = otherLoop;

                    logger.LogDebug("Nested within loop starting at {Label}", otherLoop.Header.Label);

                    break;
                }
            }
        }

        private static void FindEntryEdges(FlowgraphDfsTree dfsTree, ILogger<LoopFinder> logger, BasicBlock header, FlowGraphNaturalLoop loop)
        {
            foreach (BasicBlock predecessor in header.Predecessors)
            {
                if (dfsTree.PostOrder.Contains(predecessor) && !FlowgraphDfsTree.IsAncestor(header, predecessor))
                {
                    loop.EntryEdges.Add(new FlowEdge(predecessor, header));

                    logger.LogDebug("{PredecessorLabel} -> {HeaderLabel} is an entry edge", predecessor.Label, header.Label);
                }
            }
        }

        private static void FindExitEdges(ILogger<LoopFinder> logger, FlowGraphNaturalLoop loop)
        {
            loop.VisitLoopBlocksReversePostOrder(loopBlock =>
            {
                foreach (BasicBlock successor in loopBlock.Successors)
                {
                    if (!loop.ContainsBlock(successor))
                    {
                        FlowEdge exitEdge = new FlowEdge(loopBlock, successor);
                        loop.ExitEdges.Add(exitEdge);

                        logger.LogDebug("{LoopBlockLabel} -> {SuccessorLabel} is an exit edge", loopBlock.Label, successor.Label);
                    }
                }
                return true;
            });
        }

        private static bool FindNaturalLoopBlocks(FlowGraphNaturalLoop loop, Stack<BasicBlock> worklist, ILogger<LoopFinder> logger)
        {
            var dfsTree = loop.DfsTree;
            loop.Blocks.Add(loop.Header);

            worklist.Clear();
            foreach (FlowEdge backEdge in loop.BackEdges)
            {
                BasicBlock backEdgeSource = backEdge.Source;

                if (backEdgeSource == loop.Header)
                    continue;

                worklist.Push(backEdgeSource);
                loop.Blocks.Add(backEdgeSource);
            }

            // Work backwards through the flowgraph to the loop head or to
            // another predecessor that is outside of the loop
            while (worklist.Count != 0)
            {
                BasicBlock loopBlock = worklist.Pop();

                foreach (BasicBlock predecessorBlock in loopBlock.Predecessors)
                {
                    if (!dfsTree.PostOrder.Contains(predecessorBlock))
                        continue;

                    // The header block cannot dominate the predecessor block unless it is an ancestor
                    if (!FlowgraphDfsTree.IsAncestor(loop.Header, predecessorBlock))
                    {
                        logger.LogDebug("Loop is not natural based on {PredecessorBlockLabel} -> {LoopBlockLabel}", predecessorBlock.Label, loopBlock.Label);
                        
                        return false;
                    }

                    if (!loop.Blocks.Contains(predecessorBlock))
                    {
                        loop.Blocks.Add(predecessorBlock);
                        worklist.Push(predecessorBlock);
                    }
                }
            }

            return true;
        }
    }
}
