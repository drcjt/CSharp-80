﻿using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.Ssa;
using ILCompiler.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text;

namespace ILCompiler.Compiler
{
    public class SsaBuilder : ISsaBuilder
    {
        private readonly ILogger<SsaBuilder> _logger;

        public SsaBuilder(ILogger<SsaBuilder> logger)
        {
            _logger = logger;
        }

        public void Build(IList<BasicBlock> blocks, IList<LocalVariableDescriptor> localVariableTable)
        {
            // Topologically sort the graph
            var postOrder = TopologicalSort(blocks[0]);

            // Compute the immediate dominators of all basic blocks
            ComputeImmediateDominators(postOrder);

            // Create the dominator tree
            var dominatorTree = BuildDominatorTree(blocks);

            // Calculate liveness
            LocalVarLiveness(blocks, localVariableTable);

            // Insert Phi functions
            InsertPhiFunctions(postOrder);

            // TODO: Rename local variables

            // TODO: Print SSA form
        }

        private void InsertPhiFunctions(IList<BasicBlock> postOrderBlocks)
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
                    foreach (var blockInDominanceFront in iteratedDominanceFrontier)
                    {
                        if (!blockInDominanceFront.LiveIn.IsMember(defVar))
                        {
                            continue;
                        }

                        // TODO: Insert Phi functions
                        /*
                        if (!GetPhiNode(blockInDominanceFront, defVar))
                        {
                            InsertPhi(blockInDominanceFront, defVar);
                        }
                        */
                    }
                }
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

            var sb = new StringBuilder($"IDF({block.Label}) := {{");
            int index = 0;
            foreach (var f in iteratedDominanceFrontier)
            {
                if (index++ != 0) sb.Append(',');
                sb.Append($"{f.Label}");
            }
            sb.Append("}}");
            _logger.LogDebug(sb.ToString());

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

                var predecessors = block.Predecessors;

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

        private void LocalVarLiveness(IList<BasicBlock> blocks, IList<LocalVariableDescriptor> localVariableTable)
        {
            ClearMustInit(localVariableTable);

            // Figure out use/def info for all basic blocks
            PerBlockLocalVarLiveness(blocks);
            InterBlockLocalVarLiveness(blocks, localVariableTable);
        }

        private static void InterBlockLocalVarLiveness(IList<BasicBlock> blocks, IList<LocalVariableDescriptor> localVariableTable)
        {
            // Compute the IN and OUT sets using classic liveness algorithm
            LiveVarAnalyzer.AnalyzeLiveVars(blocks);

            // Set which local variable must be initialized
            for (var lclNum = 0; lclNum < localVariableTable.Count; lclNum++)
            {
                if (!localVariableTable[lclNum].IsParameter && blocks[0].LiveIn.IsMember(lclNum))
                {
                    localVariableTable[lclNum].MustInit = true;
                }
            }
        }

        private void PerBlockLocalVarLiveness(IList<BasicBlock> blocks)
        {
            foreach (var block in blocks)
            {
                var useSet = VariableSet.Empty;
                var defSet = VariableSet.Empty;

                // Enumerate nodes in each statement in evaluation order
                var currentNode = block.FirstNode;
                while (currentNode != null)
                {
                    PerNodeLocalVarLiveness(currentNode, useSet, defSet);
                    currentNode = currentNode.Next;
                }

                block.VarDef = defSet;
                block.VarUse = useSet;

                // Dump use def details
                var allVars = VariableSet.Union(defSet, useSet);

                _logger.LogDebug(" USE({count}={curUseVarSet}", useSet.Count, useSet.DisplayVarSet(allVars));
                _logger.LogDebug(" DEF({count}={defUseVarSet}", defSet.Count, defSet.DisplayVarSet(allVars));
            }
        }

        /// <summary>
        /// Calls MarkUseDef for any local variables encountered
        /// </summary>
        /// <param name="node"></param>
        private static void PerNodeLocalVarLiveness(StackEntry node, VariableSet useSet, VariableSet defSet)
        {
            // For LocalVariableEntry, LocalVariableAddressEntry, StoreLocalVariableEntry, StoreIndEntry??, FieldAddressEntry?
            if (node is ILocalVariable localVarNode)
            {
                MarkUseDef(localVarNode, useSet, defSet);
            }
        }

        private static void MarkUseDef(ILocalVariable tree, VariableSet useSet, VariableSet defSet)
        {
            // Assignment is a definition, everything else is a use.

            // Should we also check for StoreIndEntry which is generated from Stfld import??
            // Are these really partial definitions e.g. struct field is assigned to s.f = ...

            var isDef = tree is StoreLocalVariableEntry;
            var isUse = !isDef;

            var localNumber = tree.LocalNumber;
            if (isUse && !defSet.IsMember(localNumber))
            {
                useSet.AddElem(localNumber);
            }

            if (isDef)
            {
                defSet.AddElem(localNumber);
            }
        }

        private static void ClearMustInit(IList<LocalVariableDescriptor> localVariableTable)
        {
            foreach (var localVariable in localVariableTable)
            {
                localVariable.MustInit = false;
            }
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
                    DominatorTreeNode? node;
                    if (!nodeMap.TryGetValue(immediateDominator, out node))
                    {
                        node = new DominatorTreeNode(immediateDominator);
                        nodeMap[immediateDominator] = node;
                    }

                    var childNode = new DominatorTreeNode(block);
                    nodeMap[block] = childNode;
                    node.Children.Add(childNode);
                }
                else
                {
                    if (!nodeMap.TryGetValue(block, out rootNode))
                    {
                        rootNode = new DominatorTreeNode(block);
                        nodeMap.Add(block, rootNode);
                    }
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

        private static IList<BasicBlock> TopologicalSort(BasicBlock firstBlock)
        {
            var postOrder = new List<BasicBlock>();
            var visitedBlocks = new HashSet<BasicBlock>();

            var blocksStack = new Stack<BasicBlock>();
            var successorsStack = new Stack<int>();

            blocksStack.Push(firstBlock);
            successorsStack.Push(0);

            uint postIndex = 0;

            while (blocksStack.Count > 0)
            {
                var topBlock = blocksStack.Peek();
                var successorIndex = successorsStack.Pop();

                if (successorIndex < topBlock.Successors.Count)
                {
                    var successor = topBlock.Successors[successorIndex];
                    successorsStack.Push(successorIndex + 1);

                    if (!visitedBlocks.Contains(successor))
                    {
                        blocksStack.Push(successor);
                        successorsStack.Push(0);

                        visitedBlocks.Add(successor);
                    }
                }
                else
                {
                    var block = blocksStack.Pop();
                    block.PostOrderNum = postIndex;
                    postOrder.Add(block);
                    postIndex++;
                }
            }

            return postOrder;
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
                    foreach (var predecessor in block.Predecessors)
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
                    var basicBlockDominator = predecessorBlock;
                    foreach (var predecessor in block.Predecessors)
                    {
                        if (predecessorBlock != predecessor)
                        {
                            var dominatorAncestor = IntersectDominators(predecessor, basicBlockDominator);
                            if (dominatorAncestor != null)
                            {
                                basicBlockDominator = dominatorAncestor;
                            }
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
