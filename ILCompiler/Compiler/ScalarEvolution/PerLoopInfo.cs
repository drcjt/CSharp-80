using System.Diagnostics;
using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.ScalarEvolution
{
    internal class PerLoopInfo
    {
        FlowGraphNaturalLoops Loops { get; }

        public PerLoopInfo(FlowGraphNaturalLoops loops)
        {
            Loops = loops;
            _infos = new LoopInfo[loops.Loops.Count];
            for (int i = 0; i < _infos.Length; i++)
            {
                _infos[i] = new LoopInfo();
            }
        }

        internal record Occurrence(BasicBlock Block, Statement Stmt, LocalVariableCommon Node, Occurrence? Next);

        internal record LoopInfo
        {
            public Dictionary<int, Occurrence>? LocalToOccurrences { get; set; }
        }

        private readonly LoopInfo[] _infos;

        private readonly HashSet<BasicBlock> _visitedBlocks = [];

        public LoopInfo GetOrCreateInfo(FlowGraphNaturalLoop loop)
        {
            Debug.Assert(loop.Index <= Loops.Loops.Count);
            LoopInfo? info = _infos[loop.Index];
            Debug.Assert(info != null);

            if (info.LocalToOccurrences is null)
            {
                info.LocalToOccurrences = [];

                loop.VisitLoopBlocksReversePostOrder(block =>
                {
                    if (_visitedBlocks.Add(block))
                    {
                        SetLocalOccurrencesInfoForBlock(block, info.LocalToOccurrences);
                    }

                    return true;
                });
            }

            return info;
        }

        private static void SetLocalOccurrencesInfoForBlock(BasicBlock block, Dictionary<int, Occurrence> localToOccurrences)
        {
            foreach (Statement statement in block.Statements)
            {
                if (!statement.IsPhiDefinition)
                {
                    foreach (StackEntry node in statement.TreeList)
                    {
                        if (OperIsAnyLocal(node))
                        {
                            LocalVariableCommon nodeLocal = (LocalVariableCommon)node;
                            localToOccurrences.TryGetValue(nodeLocal.LocalNumber, out Occurrence? occurrence);

                            Occurrence newOccurrence = new(block, statement, nodeLocal, occurrence);
                            localToOccurrences[nodeLocal.LocalNumber] = newOccurrence;
                        }
                    }
                }
            }
        }

        private static bool OperIsAnyLocal(StackEntry node)
            => node is PhiArg || node is LocalVariableEntry || node is StoreLocalVariableEntry || node is LocalVariableAddressEntry;

        private bool VisitLoopNestInfo(FlowGraphNaturalLoop loop, Func<LoopInfo, bool> visitor)
        {
            for (FlowGraphNaturalLoop? child = loop.Child; child is not null; child = child.Sibling)
            {
                if (!VisitLoopNestInfo(child, visitor))
                {
                    return false;
                }
            }

            return visitor(GetOrCreateInfo(loop));
        }

        public bool VisitOccurrences(FlowGraphNaturalLoop loop, int lclNum, Func<BasicBlock, Statement, LocalVariableCommon, bool> visitor)
        {
            bool Visitor(LoopInfo info)
            {
                if (info.LocalToOccurrences!.TryGetValue(lclNum, out Occurrence? occurrence))
                {
                    do
                    {
                        if (!visitor(occurrence.Block, occurrence.Stmt, occurrence.Node))
                        {
                            return false;
                        }

                        occurrence = occurrence.Next;
                    } while (occurrence is not null);
                }

                return true;
            }

            return VisitLoopNestInfo(loop, Visitor);
        }

        public bool VisitStatementsWithOccurrences(FlowGraphNaturalLoop loop, int lclNum, Func<BasicBlock, Statement, bool> func)
        {
            bool Visitor(LoopInfo info)
            {
                if (info.LocalToOccurrences!.TryGetValue(lclNum, out Occurrence? occurrence))
                {
                    while (true)
                    {
                        if (!func(occurrence.Block, occurrence.Stmt))
                        {
                            return false;
                        }

                        Statement curStmt = occurrence.Stmt;
                        while (true)
                        {
                            occurrence = occurrence.Next;
                            if (occurrence is null)
                            {
                                return true;
                            }
                            if (occurrence.Stmt != curStmt)
                            {
                                break;
                            }
                        }
                    }
                }

                return true;
            }

            return VisitLoopNestInfo(loop, Visitor);
        }

        public void Invalidate(FlowGraphNaturalLoop loop)
        {
            for (FlowGraphNaturalLoop? child = loop.Child; child is not null; child = child.Sibling)
            {
                Invalidate(child);
            }

            LoopInfo info = _infos[loop.Index];
            if (info.LocalToOccurrences is not null)
            {
                info.LocalToOccurrences = null;
                _visitedBlocks.Clear();
            }
        }
    }
}
