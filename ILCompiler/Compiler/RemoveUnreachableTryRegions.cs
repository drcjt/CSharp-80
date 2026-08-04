using ILCompiler.Compiler.FlowgraphHelpers;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler
{
    internal class RemoveUnreachableTryRegoins : IRemoveUnreachableTryRegions
    {
        public void Run(MethodCompiler compiler)
        {
            FlowgraphDfsTree dfsTree = compiler.DfsTree!;

            if (compiler.ControlFlowGraph.EhClauses.Count == 0)
            {
                // No EH handling in this method nothing to do
                return;
            }

            // PASS 1: directly dead regions (try entry not in DFS)
            List<EHClause> deadEHClauses = [.. compiler.ControlFlowGraph.EhClauses.Where(c => !dfsTree.PostOrder.Contains(c.TryBegin))];

            // Close under structural containment.
            IList<EHClause> containedDeadClauses = compiler.ControlFlowGraph.EhClauses
                .Where(c => deadEHClauses.Any(parent =>
                    c != parent &&
                    c.TryBegin.StartOffset >= parent.TryBegin.StartOffset &&
                    c.HandlerLast.EndOffset <= parent.HandlerLast.EndOffset))
                .ToList();

            deadEHClauses.AddRange(containedDeadClauses.Where(c => !deadEHClauses.Contains(c)));

            // PASS 2: Determine the blocks to remove based on the dead EH clauses
            IList<BasicBlock> blocksToRemove = [];
            foreach (EHClause deadClause in deadEHClauses)
            {
                compiler.ControlFlowGraph.EhClauses.Remove(deadClause);

                foreach (BasicBlock block in compiler.ControlFlowGraph.Blocks)
                {
                    if (block.StartOffset >= deadClause.TryBegin.StartOffset && block.EndOffset <= deadClause.HandlerLast.EndOffset)
                    {
                        blocksToRemove.Add(block);
                    }
                }
            }

            // PASS 3: Remove the unreachable blocks
            foreach (BasicBlock block in blocksToRemove)
            {
                compiler.ControlFlowGraph.Remove(block);
            }
        }
    }
}
