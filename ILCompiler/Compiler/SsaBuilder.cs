﻿using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.FlowgraphHelpers;
using ILCompiler.Compiler.Ssa;
using ILCompiler.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text;

namespace ILCompiler.Compiler
{
    // TODO:
    // StoreInd - treat as a store??

    public class SsaBuilder : ISsaBuilder
    {
        private readonly ILogger<SsaBuilder> _logger;
        private bool _dumpSsa;

        public SsaBuilder(ILogger<SsaBuilder> logger)
        {
            _logger = logger;
        }

        public void Build(IList<BasicBlock> blocks, LocalVariableTable locals, bool dumpSsa)
        {
            _dumpSsa = dumpSsa;

            blocks = SetupBasicBlockRoot(blocks);

            // Topologically sort the graph
            var postOrder = FlowgraphDfsTree.Build(blocks[0]).PostOrder;

            // Compute the immediate dominators of all basic blocks
            ComputeImmediateDominators(postOrder);

            // Create the dominator tree
            var dominatorTree = BuildDominatorTree(blocks);

            // Calculate liveness
            Liveness.LocalVarLiveness(blocks, locals, _logger);

            // Calculate Ssa only for Tracked local variables
            foreach (var localVariable in locals) 
            {
                localVariable.InSsa = localVariable.Tracked;
            }

            // Insert Phi functions
            InsertPhiFunctions(postOrder, locals);

            // Rename local variables
            RenameVariables(dominatorTree, locals);

            // Log SSA form
            if (_dumpSsa)
            {
                LogSsaSummary(locals);
            }
        }

        private static IList<BasicBlock> SetupBasicBlockRoot(IList<BasicBlock> blocks) 
        {
            if (blocks[0].Predecessors.Count != 0)
            {
                // Need to create a new basic block to act as the loop as the real first block is a loop
                var basicBlockRoot = new BasicBlock(0);
                blocks.Insert(0, basicBlockRoot);
            }

            return blocks;
        }

        private void LogSsaSummary(LocalVariableTable locals)
        {
            for (var localNumber = 0; localNumber < locals.Count; localNumber++)
            {
                var localVariableDescriptor = locals[localNumber];
                if (localVariableDescriptor.InSsa)
                {
                    var ssaDefinitions = localVariableDescriptor.PerSsaData;
                    var numDefinitions = ssaDefinitions.Count;

                    if (numDefinitions == 0)
                    {
                        _logger.LogInformation("V{localNumber:00} in SSA but no definitions", localNumber);
                    }
                    else
                    {
                        LogSsaSumaryDefinitions(localNumber, ssaDefinitions, numDefinitions);
                    }
                }
            }
        }

        private void LogSsaSumaryDefinitions(int localNumber, SsaDefList<LocalSsaVariableDescriptor> ssaDefinitions, int numDefinitions)
        {
            for (var defIndex = 0; defIndex < numDefinitions; defIndex++)
            {
                var ssaVarDefinition = ssaDefinitions.SsaDefinitionByIndex(defIndex);
                var ssaNumber = ssaDefinitions.GetSsaNumber(ssaVarDefinition);
                var block = ssaVarDefinition.Block;

                _logger.LogInformation("V{localNumber:00}.{ssaNumber:00}: defined in {blockLabel} {uses} uses {useType}",
                    localNumber, ssaNumber, block.Label, ssaVarDefinition.NumberOfUses, ssaVarDefinition.HasGlobalUse ? "global" : "local");
            }
        }

        private void RenameVariables(DominatorTreeNode tree, LocalVariableTable locals)
        {
            var ssaRenameStack = new SsaRenameState(_logger, locals.Count);

            // First deal with parameters and must-init variables as though they
            // have a virtual definition before entry to the method. They all
            // begin with a SSA number of 1.
            for (int localVariableNumber = 0; localVariableNumber < locals.Count; localVariableNumber++)
            {
                var localVariableDescriptor = locals[localVariableNumber];
                if (localVariableDescriptor.IsParameter || localVariableDescriptor.MustInit || 
                    localVariableDescriptor.Type.IsGC())
                {
                    var ssaNumber = localVariableDescriptor.PerSsaData.AllocSsaNumber(() => new LocalSsaVariableDescriptor(tree.Block));
                    ssaRenameStack.Push(tree.Block, localVariableNumber, ssaNumber);
                }
            }

            /*
             * stack[v] is a stack of variable names (for every variable v)

             * def rename(block):
             *   for instr in block: (BlockRenameVariables in Ryujit)
             *     replace each argument to instr with stack[old name]

             *     replace instr's destination with a new name
             *     push that new name onto stack[old name]

             *   for s in block's successors: (AddPhiArgsToSuccessors in Ryujit)
             *     for p in s's ϕ-nodes:
             *       Assuming p is for a variable v, make it read from stack[v].

             *   for b in blocks immediately dominated by block: (DomTreeVisitor in Ryujit)
             *     # That is, children in the dominance tree.
             *     rename(b)

             *   pop all the names we just pushed onto the stacks (PopBlockStacks in Ryujit)

             * rename(entry)
             */

            var visitor = new SsaRenameDominatorTreeVisitor(tree, ssaRenameStack, locals);
            visitor.WalkTree();
        }

        private void InsertPhiFunctions(IList<BasicBlock> postOrderBlocks, LocalVariableTable locals)
        {
            // From https://www.cs.cornell.edu/courses/cs6120/2020fa/lesson/5/

            // for v in vars:
            //    for d in Defs[v]:  # Blocks where v is assigned.
            //       for block in DF[d]:  # Dominance frontier.
            //          Add a ϕ - node to block,
            //            unless we have done so already.
            //          Add block to Defs[v](because it now writes to v!),
            //            unless it's already in there.

            // Compute the Dominance frontiers
            var dominanceFrontierMap = ComputeDominanceFrontiers(postOrderBlocks);

            for (int i = 0; i < postOrderBlocks.Count; i++)
            {
                var block = postOrderBlocks[i];

                var iteratedDominanceFrontier = ComputeIteratedDominanceFrontier(block, dominanceFrontierMap);

                if (iteratedDominanceFrontier.Count == 0)
                {
                    continue;
                }

                // Loop over variables that are assigned to in the block e.g. the def's
                foreach (var defVar in block.VarDef)
                {
                    if (!locals[defVar].InSsa)
                        continue;

                    foreach (var blockInDominanceFront in iteratedDominanceFrontier)
                    {
                        if (!blockInDominanceFront.LiveIn.IsMember(defVar))
                        {
                            continue;
                        }

                        // Insert Phi functions - note arguments will be determined during variable renaming
                        if (!HasPhiNode(blockInDominanceFront, defVar))
                        {
                            InsertPhi(blockInDominanceFront, defVar, locals);
                        }
                    }
                }
            }
        }

        private static bool HasPhiNode(BasicBlock block, int localNumber)
        {
            foreach (var statement in block.Statements)
            {
                if (statement.RootNode is StoreLocalVariableEntry store && store.Op1 is PhiNode)
                {
                    if (store.LocalNumber == localNumber)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void InsertPhi(BasicBlock block, int localNumber, LocalVariableTable locals)
        {
            // Note actual arguments to Phi will be worked out during variable renaming

            var local = locals[localNumber];
            var phiNode = new PhiNode(local.Type);

            var store = new StoreLocalVariableEntry(localNumber, false, phiNode);

            // Create the statement and chain together in linear order e.g. PhiNode followed by StoreLocalVariableEntry
            var statement = new Statement(store)
            {
                TreeList = new List<StackEntry> { phiNode, store }
            };
            phiNode.Next = store;
            store.Prev = phiNode;

            // Add new store/phi statement to start of block
            block.Statements.Insert(0, statement);

            if (_dumpSsa)
            {
                _logger.LogInformation("Added Phi Node for V{localNumber} at start of {blockLabel}.", localNumber, block.Label);
            }
        }

        private IList<BasicBlock> ComputeIteratedDominanceFrontier(BasicBlock block, IDictionary<BasicBlock, ISet<BasicBlock>> dominanceFrontierMap)
        {
            // The iterated dominance frontier of block B is the smallest set that includes
            // B's dominance frontier and also includes the dominance frontier of all elements
            // of the set

            var iteratedDominanceFrontier = new List<BasicBlock>();

            if (dominanceFrontierMap.TryGetValue(block, out var dominanceFrontier))
            {
                // Start by adding the dominance frontier of block to the iterated dominance frontier
                iteratedDominanceFrontier.AddRange(dominanceFrontier);

                for (var i = 0; i < iteratedDominanceFrontier.Count; i++)
                {
                    var f = iteratedDominanceFrontier[i];

                    if (dominanceFrontierMap.TryGetValue(f, out var fDominanceFrontier))
                    {
                        foreach (var ff in fDominanceFrontier)
                        {
                            if (!iteratedDominanceFrontier.Contains(ff))
                            {
                                iteratedDominanceFrontier.Add(ff);
                            }
                        }
                    }
                }
            }

            if (_dumpSsa)
            {
                var sb = new StringBuilder($"IDF({block.Label}) := {{");
                int index = 0;
                foreach (var f in iteratedDominanceFrontier)
                {
                    if (index++ != 0) sb.Append(',');
                    sb.Append($"{f.Label}");
                }
                sb.Append("}}");
                _logger.LogInformation(sb.ToString());
            }

            return iteratedDominanceFrontier;
        }

        private IDictionary<BasicBlock, ISet<BasicBlock>> ComputeDominanceFrontiers(IList<BasicBlock> postOrderBlocks)
        {
            // From "A simple, fast dominance algorithm", by Cooper, Harvey, and Kennedy.

            // Recall that the dominance frontier of a block B is the set of blocks
            // B3 such that there exists some B2 s.t. B3 is a successor of B2, and
            // B dominates B2. Note that this dominance need not be strict -- B2
            // and B may be the same node.

            var dominanceFrontierMap = new Dictionary<BasicBlock, ISet<BasicBlock>>();

            _logger.LogDebug("Computing Dominance Frontiers");

            for (int i = 0; i < postOrderBlocks.Count; i++)
            {
                var block = postOrderBlocks[i];

                _logger.LogDebug("Considering block {BlockLabel}", block.Label);

                // Block B3 is in the dominance frontier of B1 if there is a B2
                // such that B1 dominates B2 and B1 does not dominate B3 and B3
                // is an immediate successor of B2.
                // Note that B1 might be the same block as B2

                // Here block is B3, and we are trying to find the B1's

                // First we consider the predecessors of block looking for candidate B2's
                // block is obviously an immediate successor of its immediate predecessors

                // If block is first block in exception handler then use all blocks handled by the handler
                IList<BasicBlock> predecessors = !block.HandlerStart ? block.Predecessors : block.TryBlocks;

                // If there is zero or one predecessors then there is no predecessor,
                // or else the single predecessor dominates block, so no B2 exists
                if (predecessors.Count <= 1)
                {
                    _logger.LogDebug("   Has {predecessorCount} predecessors; skipping", predecessors.Count);
                    continue;
                }

                // We have more than one predecessor, each is a potential candidate for B2
                // unless it dominates block

                foreach (var predecessor in predecessors)
                {
                    // For each B2 consider the possible B1's by starting with B2 since a block dominates itself.
                    // Then traverse upwards in the dominator tree stopping if we reach the root or the
                    // immediate dominator of block
                    // Along the way we make block part of the dominance frontier of B1
                    for (var b1 = predecessor; b1 != null && b1 != block.ImmediateDominator; b1 = b1.ImmediateDominator)
                    {
                        _logger.LogDebug("      Adding {b1Label} to dominance frontier of predecessor dominator {predecessorDominator}", block.Label, b1.Label);

                        if (!dominanceFrontierMap.TryGetValue(b1, out var dominanceFrontierBlocks))
                        {
                            dominanceFrontierBlocks = new HashSet<BasicBlock>();
                            dominanceFrontierMap[b1] = dominanceFrontierBlocks;
                        }

                        dominanceFrontierBlocks.Add(block);
                    }
                }
            }

            _logger.LogDebug("Computed Dominance Frontier");
            for (int i = 0; i < postOrderBlocks.Count; i++)
            {
                var block = postOrderBlocks[i];

                var sb = new StringBuilder($"Block {block.Label} := {{");

                if (dominanceFrontierMap.TryGetValue(block, out var dominanceFrontier))
                {
                    var index = 0;
                    foreach (var dominanceFrontierBlock in dominanceFrontier)
                    {
                        if (index++ != 0) sb.Append(',');
                        sb.Append($"{dominanceFrontierBlock.Label}");
                    }
                }
                sb.Append("}}");
                _logger.LogDebug(sb.ToString());
            }

            return dominanceFrontierMap;
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

        private DominatorTreeNode BuildDominatorTree(IList<BasicBlock> blocks)
        {
            _logger.LogDebug("[SsaBuilder:BuildDominatorTree])");

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

        private void ComputeImmediateDominators(IList<BasicBlock> postOrder)
        {
            _logger.LogDebug("[SsaBuilder:ComputeImmediateDominators])");

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
                            basicBlockDominator = IntersectDominators(basicBlockDominator, domPred);
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
        /// so we use the predecessors of the first block in the try
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
                return block.TryBlocks[0].Predecessors;
            }
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
