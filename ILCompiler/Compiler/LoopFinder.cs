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

        public void FindLoops(IList<BasicBlock> blocks)
        {
            var dfs = FlowgraphDfsTree.Build(blocks[0]);

            var loops = FlowGraphNaturalLoops.Find(dfs, _logger);

            // Consider moving out of here
            ComputeReachabilitySets(dfs);
        }

        private static void ComputeReachabilitySets(FlowgraphDfsTree dfs)
        {
            var reversePostOrder = dfs.PostOrder.Reverse();

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

    public class FlowGraphNaturalLoop(FlowgraphDfsTree dfsTree, BasicBlock header)
    {
        public FlowgraphDfsTree DfsTree { get; init; } = dfsTree;
        public BasicBlock Header { get; } = header;
        public IList<FlowEdge> BackEdges { get; } = [];
        public IList<FlowEdge> ExitEdges { get; } = [];
        public IList<FlowEdge> EntryEdges { get; } = [];
        public FlowGraphNaturalLoop? Parent { get; set; }

        public IList<BasicBlock> Blocks { get; } = [];

        public bool VisitLoopBlocksReversePostOrder(Func<BasicBlock, bool> visitLoopBlock)
        {
            foreach (var block in Blocks.OrderByDescending(x => x.PostOrderNum))
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
    }

    public class FlowGraphNaturalLoops()
    {
        private readonly IList<FlowGraphNaturalLoop> _loops = [];

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
                var rpoNumber = dfsTree.PostOrder.Count - i;
                var block = dfsTree.PostOrder[i-1];
                logger.LogDebug($"{rpoNumber} -> {block.Label} [{block.PreOrderNum}, {block.PostOrderNum}]");
            }

            var loops = new FlowGraphNaturalLoops();

            // Check if tree has any cycles if not skip
            if (!dfsTree.HasCycle)
            {
                logger.LogDebug("Flow graph has no cycles. Skipping identification of natural loops");

                return loops;
            }

            foreach (var header in dfsTree.PostOrder.Reverse())
            {
                FlowGraphNaturalLoop? loop = null;

                // If a block is a ancestor of one of its predecessors then the block is a loop header
                loop = FindBackEdges(dfsTree, logger, header, loop);

                if (loop == null)
                    continue;

                logger.LogDebug($"{header.Label} is the header of a DFS loop with {loop.BackEdges.Count} backedges");

                // Walk backwards in flow along the back edges from header to determine if this 
                // is a natural loop and to find all of the basic blocks in the loop
                var worklist = new Stack<BasicBlock>();
                if (!FindNaturalLoopBlocks(loop, worklist, logger))
                {
                    // Improper loop
                    logger.LogDebug("Loop contains an improper loop header");
                    continue;
                }

                logger.LogDebug($"Loop has {loop.Blocks.Count} blocks");

                // Find the exit edges
                FindExitEdges(logger, loop);

                // Find the entry edges
                FindEntryEdges(dfsTree, logger, header, loop);

                // Search for parent loop
                FindParentLoop(logger, loops, header, loop);

                loops.Add(loop);

                logger.LogDebug($"Added loop with header {loop.Header.Label}");
            }

            return loops;
        }

        private static FlowGraphNaturalLoop? FindBackEdges(FlowgraphDfsTree dfsTree, ILogger<LoopFinder> logger, BasicBlock header, FlowGraphNaturalLoop? loop)
        {
            foreach (var predecessorBlock in header.Predecessors)
            {
                if (dfsTree.PostOrder.Contains(predecessorBlock) && FlowgraphDfsTree.IsAncestor(header, predecessorBlock))
                {
                    loop = loop ?? new FlowGraphNaturalLoop(dfsTree, header);
                    loop.BackEdges.Add(new FlowEdge(predecessorBlock, header));

                    logger.LogDebug($"{predecessorBlock.Label} -> {header.Label} is a backedge");
                }
            }

            return loop;
        }

        private static void FindParentLoop(ILogger<LoopFinder> logger, FlowGraphNaturalLoops loops, BasicBlock header, FlowGraphNaturalLoop loop)
        {
            foreach (var otherLoop in loops.InPostOrder)
            {
                if (otherLoop.ContainsBlock(header))
                {
                    loop.Parent = otherLoop;

                    logger.LogDebug($"Nested within loop starting at {otherLoop.Header.Label}");

                    break;
                }
            }
        }

        private static void FindEntryEdges(FlowgraphDfsTree dfsTree, ILogger<LoopFinder> logger, BasicBlock header, FlowGraphNaturalLoop loop)
        {
            foreach (var predecessor in header.Predecessors)
            {
                if (dfsTree.PostOrder.Contains(predecessor) && FlowgraphDfsTree.IsAncestor(header, predecessor))
                {
                    loop.EntryEdges.Add(new FlowEdge(predecessor, header));

                    logger.LogDebug($"{predecessor.Label} -> {header.Label} is an entry edge");
                }
            }
        }

        private static void FindExitEdges(ILogger<LoopFinder> logger, FlowGraphNaturalLoop loop)
        {
            loop.VisitLoopBlocksReversePostOrder(loopBlock =>
            {
                foreach (var successor in loopBlock.Successors)
                {
                    if (!loop.ContainsBlock(successor))
                    {
                        FlowEdge exitEdge = new FlowEdge(loopBlock, successor);
                        loop.ExitEdges.Add(exitEdge);

                        logger.LogDebug($"{loopBlock.Label} -> {successor.Label} is an exit edge");
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
            foreach (var backEdge in loop.BackEdges)
            {
                var backEdgeSource = backEdge.Source;

                if (backEdgeSource == loop.Header)
                    continue;

                worklist.Push(backEdgeSource);
                loop.Blocks.Add(backEdgeSource);
            }

            // Work backwards through the flowgraph to the loop head or to
            // another predecessor that is outside of the loop
            while (worklist.Count != 0)
            {
                var loopBlock = worklist.Pop();

                foreach (var predecessorBlock in loopBlock.Predecessors)
                {
                    if (!dfsTree.PostOrder.Contains(predecessorBlock))
                        continue;

                    // The header block cannot dominate the predecessor block unless it is an ancestor
                    if (!FlowgraphDfsTree.IsAncestor(loop.Header, predecessorBlock))
                    {
                        logger.LogDebug($"Loop is not natural based on {predecessorBlock.Label} -> {loopBlock.Label}");
                        
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