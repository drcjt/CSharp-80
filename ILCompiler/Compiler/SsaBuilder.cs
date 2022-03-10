using ILCompiler.Compiler.EvaluationStack;
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

            // TODO: Calculate liveness
            LocalVarLiveness(blocks, localVariableTable);

            // TODO: Insert Phi functions

            // TODO: Rename local variables

            // TODO: Print SSA form
        }

        private void LocalVarLiveness(IList<BasicBlock> blocks, IList<LocalVariableDescriptor> localVariableTable)
        {
            LocalVarLivenessInit(localVariableTable);

            InitBlockVarSets(blocks);

            // Figure out use/def info for all basic blocks
            PerBlockLocalVarLiveness(blocks, localVariableTable);
            // InterBLockLocalVarLiveness()
        }

        private void PerBlockLocalVarLiveness(IList<BasicBlock> blocks, IList<LocalVariableDescriptor> localVariableTable)
        {
            foreach (var block in blocks)
            {
                // clear current use var set
                // clear current def var set

                // Enumerate nodes in each statement in evaluation order
                foreach (var stmt in block.Statements)
                {
                    for (var node = stmt; node != null; node = node.Next)
                    {
                        PerNodeLocalVarLiveness(node, localVariableTable);
                    }
                }

                // Save current use var set as block's use var set
                // Save current def var set as block's def var set
            }
        }

        /// <summary>
        /// Calls MarkUseDef for any local variables encountered
        /// </summary>
        /// <param name="node"></param>
        private void PerNodeLocalVarLiveness(StackEntry node, IList<LocalVariableDescriptor> localVariableTable)
        {
            // For LocalVariableEntry, LocalVariableAddressEntry, StoreLocalVariableEntry, StoreIndEntry??, FieldAddressEntry?
            if (node is ILocalVariable)
            {
                var localVarNode = (ILocalVariable)node;
                MarkUseDef(localVarNode, localVariableTable);
            }
        }

        private void MarkUseDef(ILocalVariable tree, IList<LocalVariableDescriptor> localVariableTable)
        {
            var localNumber = tree.LocalNumber;
            var local = localVariableTable[localNumber];

            // Need to get def/use from tree too
            // So assignment is a definition, everything else is a use.

            // if (isUse && not in current def set)
            //   Add to Current Use set

            // if (isDef)
            //   Add to Current Def set
        }

        private void InitBlockVarSets(IList<BasicBlock> blocks)
        {
            foreach (var block in blocks)
            {
                // block.InitVarSets();
            }
        }

        private void LocalVarLivenessInit(IList<LocalVariableDescriptor> localVariableTable)
        {
            foreach (var localVariable in localVariableTable)
            {
                localVariable.MustInit = false;
            }
        }

        // TODO: Move to separate file
        // TODO: Will need ability to do a pre order traversal of this tree
        class DominatorTreeNode
        {
            public BasicBlock Block { get; private set; }
            public IList<DominatorTreeNode> Children { get; set; } = new List<DominatorTreeNode>();

            public DominatorTreeNode(BasicBlock block)
            {
                Block = block;
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

            var processedBlocks = new HashSet<BasicBlock>();
            processedBlocks.Add(postOrder[postOrder.Count - 1]);

            bool changed = true;
            while (changed)
            {
                changed = false;

                // In reverse post order except for the entry block, (count - 1) is the entry block
                for (int i = count - 2; i >= 0; --i)
                {
                    var block = postOrder[i];

                    _logger.LogDebug($"Visiting in reverse post order: {block.Label}");

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
                        _logger.LogDebug($"Predecessor block is {predecessorBlock.Label}");
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

                        _logger.LogDebug($"ImmediateDominator of {block.Label} becomes {basicBlockDominator?.Label}");
                        block.ImmediateDominator = basicBlockDominator;
                    }

                    _logger.LogDebug($"Marking block {block.Label} as processed");
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
