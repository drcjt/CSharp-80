using System.Diagnostics;
using ILCompiler.Compiler.Dominators;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.FlowgraphHelpers;
using ILCompiler.Compiler.Helpers;
using ILCompiler.Compiler.ScalarEvolution;
using ILCompiler.Interfaces;
using Microsoft.Extensions.Logging;

namespace ILCompiler.Compiler
{
    internal class InductionVariableOptimizer : IInductionVariableOptimizer
    {
        private readonly ILogger<InductionVariableOptimizer> _logger;
        private readonly IFlowgraph _flowgraph;

        public InductionVariableOptimizer(ILogger<InductionVariableOptimizer> logger, IFlowgraph flowgraph)
        {
            _logger = logger;
            _flowgraph = flowgraph;
        }

        public void Run(IList<BasicBlock> blocks, FlowGraphNaturalLoops loops, LocalVariableTable locals)
        {
            if (loops.Loops.Count > 0)
            {
                PerLoopInfo loopInfo = new PerLoopInfo(loops);

                _logger.LogInformation("Optimizing induction variables");

                ScevContext scalarEvolutionContext = new(locals);
                foreach (FlowGraphNaturalLoop? loop in loops.InPostOrder.Reverse())
                {
                    scalarEvolutionContext.ResetForLoop(loop);
                    // For now, just log the loops we found
                    // Consider doing something useful here
                    _logger.LogInformation("Processing loop with header {LoopLabel}", loop.Header.Label);

                    var strengthReductionContext = new StrengthReductionContext(scalarEvolutionContext, loop, loopInfo, locals, _flowgraph, _logger);
                    strengthReductionContext.TryStrengthReduce();

                    // TODO: Remove unused IVs
                }
            }
        }

        public static bool LocalHasNonLoopUses(int lclNum, FlowGraphNaturalLoop loop, PerLoopInfo loopInfo)
        {
            bool visitResult = loop.VisitRegularExitBlocks(block =>
            {
                return !LocalIsLiveIntoBlock(lclNum, block);
            });

            if (!visitResult)
            {
                return true;
            }
            return false;
        }

        private static bool LocalIsLiveIntoBlock(int lclNum, BasicBlock block)
        {
            return block.LiveIn.IsMember(lclNum);
        }

        private static bool IsUpdateOfIVWithoutSideEffects(StackEntry tree, int localNumber)
        {
            if (tree is not StoreLocalVariableEntry)
            {
                return false;
            }
            var store = (LocalVariableCommon)tree;
            if (store.LocalNumber != localNumber)
            {
                return false;
            }

            // TODO: Check for side effects

            return true;
        }

        internal class StrengthReductionContext
        {
            ScevContext ScalarEvolutionContext { get; }
            FlowGraphNaturalLoop Loop { get; }
            PerLoopInfo LoopInfo { get; }

            Stack<CursorInfo> Cursors1 { get; } = new Stack<CursorInfo>();
            Stack<CursorInfo> Cursors2 { get; } = new Stack<CursorInfo>();
            Stack<CursorInfo> IntermediateIVStores { get; } = new Stack<CursorInfo>();

            private readonly LocalVariableTable _locals;
            private readonly IFlowgraph _flowgraph;

            private readonly ILogger<InductionVariableOptimizer> _logger;

            public StrengthReductionContext(ScevContext scalarEvolutionContext, FlowGraphNaturalLoop loop, PerLoopInfo loopInfo, LocalVariableTable locals,
                IFlowgraph flowgraph, ILogger<InductionVariableOptimizer> logger)
            {
                ScalarEvolutionContext = scalarEvolutionContext;
                Loop = loop;
                LoopInfo = loopInfo;
                _logger = logger;
                _locals = locals;
                _flowgraph = flowgraph;
            }

            public bool TryStrengthReduce()
            {
                bool strengthReducedAny = false;

                _logger.LogInformation("Considering L{LoopIndex} for strength reduction...", Loop.Index);
                _logger.LogInformation("   Considering primary IVs");

                foreach (var statement in Loop.Header.Statements)
                {
                    if (!statement.IsPhiDefinition)
                    {
                        break;
                    }

                    LocalVariableCommon primaryIVLcl = (LocalVariableCommon)statement.RootNode;
                    Scev? candidate = ScalarEvolutionContext.Analyze(Loop.Header, primaryIVLcl.Data);

                    if (candidate is null)
                    {
                        _logger.LogInformation("   Could not analyse loop header PHI");
                        continue;
                    }

                    // Simplify
                    candidate = candidate.Simplify(ScalarEvolutionContext);

                    _logger.LogInformation("   => ");

                    if (candidate is not ScevAddRec)
                    {
                        _logger.LogInformation("   Not an addrec");
                        continue;
                    }

                    // TODO: check for non-loop uses

                    ScevAddRec primaryIV = (ScevAddRec)candidate;

                    if (!InitializeCursors(primaryIVLcl, primaryIV))
                    {
                        continue;
                    }

                    Stack<CursorInfo> cursors = Cursors1;
                    (int derivedLevel, ScevAddRec? currentIV) = WalkCursors(primaryIV, ref cursors);
                    if (derivedLevel == 0)
                    {
                        continue;
                    }

                    _logger.LogInformation("   All uses of primary IV {LocalNumber} are used to compute a {DerivedLevel}-derived IV", primaryIVLcl.LocalNumber, derivedLevel);

                    Debug.Assert(currentIV is not null);

                    if (currentIV.Step == primaryIV.Step)
                    {
                        _logger.LogInformation("   Skipping: Candidate has same step as primary");
                        continue;
                    }

                    if (TryReplaceUsesWithNewPrimaryIV(cursors, currentIV))
                    {
                        strengthReducedAny = true;
                        LoopInfo.Invalidate(Loop);
                    }
                }

                return strengthReducedAny;
            }

            private (int derivedLevel, ScevAddRec? currentIV) WalkCursors(ScevAddRec primaryIV, ref Stack<CursorInfo> cursors)
            {
                ScevAddRec? currentIV = primaryIV;
                Stack<CursorInfo> nextCursors = Cursors2;

                int derivedLevel = 0;

                while (true)
                {
                    _logger.LogInformation("   Advancing cursors to be {DerivedLevel}", derivedLevel + 1);

                    AdvanceCursors(cursors, nextCursors);

                    if (!CheckAdvancedCursors(nextCursors, out ScevAddRec? nextIV))
                    {
                        break;
                    }

                    _logger.LogInformation("   Next IV is: {NextIV}", nextIV);

                    ExpandStoredCursors(nextCursors, cursors);

                    derivedLevel++;

                    (cursors, nextCursors) = (nextCursors, cursors);
                    currentIV = nextIV;
                }

                return (derivedLevel, currentIV);
            }

            private void ExpandStoredCursors(Stack<CursorInfo> cursors, Stack<CursorInfo> otherCursors)
            {
                for (int i = 0; i < cursors.Count; i++)
                {
                    while (true)
                    {
                        CursorInfo? cursor = cursors.Bottom(i);
                        StackEntry cur = cursor.Tree!;

                        StackEntry? parent = cur.GetParent(out _);
                        if (parent is null || (parent is CommaEntry commaEntry && commaEntry.Op1 == cur))
                        {
                            break;
                        }

                        if (parent is StoreLocalVariableEntry)
                        {
                            ExpandStoreLocalVariableEntry(cursors, otherCursors, ref i, cursor, cur, parent);
                            break;
                        }

                        Scev? parentIV = ScalarEvolutionContext.Analyze(cursor.Block, parent);
                        if (parentIV is null)
                        {
                            break;
                        }

                        parentIV = parentIV.Simplify(ScalarEvolutionContext);
                        Debug.Assert(parentIV is not null);
                        if (parentIV != cursor.IV)
                        {
                            break;
                        }

                        cursor.Tree = parent;
                    }
                }
            }

            private void ExpandStoreLocalVariableEntry(Stack<CursorInfo> cursors, Stack<CursorInfo> otherCursors, ref int i, CursorInfo cursor, StackEntry cur, StackEntry parent)
            {
                LocalVariableCommon storedLcl = (LocalVariableCommon)parent;
                // TODO: side effects and LocalHasNonLoopUses
                if (storedLcl.Data == cur)
                {
                    int numCreated = 0;
                    ScevAddRec? cursorIV = cursor.IV;
                    BasicBlock cursorBlock = cursor.Block;
                    Statement cursorStmt = cursor.Stmt;

                    bool CreateExtraCursor(BasicBlock block, Statement stmt, LocalVariableCommon use)
                    {
                        if (use == parent)
                        {
                            return true;
                        }

                        if (use is not LocalVariableEntry || use.SsaNumber != storedLcl.SsaNumber)
                        {
                            return false;
                        }

                        Scev? iv = ScalarEvolutionContext.Analyze(block, use);
                        if (iv is null)
                        {
                            return false;
                        }

                        iv = iv.Simplify(ScalarEvolutionContext);
                        if (iv != cursorIV)
                        {
                            return false;
                        }

                        cursors.Push(new CursorInfo(block, stmt, use, cursorIV));
                        otherCursors.Push(new CursorInfo(block, stmt, use, cursorIV));
                        numCreated++;
                        return true;
                    }

                    if (LoopInfo.VisitOccurrences(Loop, storedLcl.LocalNumber, CreateExtraCursor))
                    {
                        _logger.LogInformation("   [{CurrentTreeID}] was the data of store [{ParentTreeID}]; expanded to {NumCreated} new cursors and will replace with a store of 0", cur.TreeID, parent.TreeID, numCreated + 1);

                        IntermediateIVStores.Push(new CursorInfo(cursorBlock, cursorStmt, parent, null));
                        cursors.SwapTopWithBottom(i);
                        otherCursors.SwapTopWithBottom(i);
                        cursors.Pop();
                        otherCursors.Pop();
                        i--;
                        return;
                    }

                    cursors.Pop(numCreated);
                    otherCursors.Pop(numCreated);
                }
            }

            private bool TryReplaceUsesWithNewPrimaryIV(Stack<CursorInfo> cursors, ScevAddRec iv)
            {
                int? stepConstant = iv.Step.GetConstantValue();
                if (stepConstant is null)
                {
                    _logger.LogInformation("   Skipping: step value is not a constant");
                    return false;
                }

                BasicBlock? insertionPoint = FindUpdateInsertionPoint(cursors);
                if (insertionPoint is null)
                {
                    _logger.LogInformation("   Skipping: could not find a legal insertion point for the new IV update");
                    return false;
                }

                BasicBlock preHeader = Loop.EntryEdges[0].Source;
                StackEntry? initValue = ScevContext.Materialize(iv.Start);
                if (initValue is null)
                {
                    _logger.LogInformation("   Skipping: could not materialize initial value");
                    return false;
                }

                _logger.LogInformation("   Strength reducing");

                StackEntry? stepValue = ScevContext.Materialize(iv.Step);
                Debug.Assert(stepValue is not null);

                TreeDumper dumper = new TreeDumper();

                int newPrimaryIV = _locals.GrabTemp(iv.Type, iv.Type.GetTypeSize());
                StackEntry initStore = NewTempStore(newPrimaryIV, initValue);
                Statement initStmt = NewStmtFromTree(initStore);

                _flowgraph.InsertStatementNearEnd(preHeader, initStmt);

                _logger.LogInformation("   Inserting init statement in preheader {PreHeaderLabel}", preHeader.Label);
                _logger.LogInformation(dumper.Dump(initStmt));

                StackEntry nextValue = new BinaryOperator(Operation.Add, false, new LocalVariableEntry(newPrimaryIV, iv.Type, iv.Type.GetTypeSize()), stepValue, iv.Type);
                StackEntry stepStore = NewTempStore(newPrimaryIV, nextValue);
                Statement stepStmt = NewStmtFromTree(stepStore);

                _flowgraph.InsertStatementNearEnd(insertionPoint, stepStmt);

                _logger.LogInformation("   Inserting step statement in {InsertionPointLabel}", insertionPoint.Label);
                _logger.LogInformation(dumper.Dump(stepStmt));

                // replace uses
                for (int i = 0; i < cursors.Count; i++)
                {
                    CursorInfo cursor = cursors.Bottom(i);
                    StackEntry newUse = new LocalVariableEntry(newPrimaryIV, iv.Type, iv.Type.GetTypeSize());
                    newUse = RephraseIV(cursor.IV!, iv, newUse);

                    _logger.LogInformation("   Replacing use {OriginalTreeID} with {NewTreeID}. Before:", cursor.Tree!.TreeID, newUse.TreeID);
                    _logger.LogInformation(dumper.Dump(cursor.Stmt));

                    Edge<StackEntry>? use;
                    if (cursor.Stmt.RootNode == cursor.Tree)
                    {
                        use = new Edge<StackEntry>(() => cursor.Stmt.RootNode, x => { cursor.Stmt.RootNode = x; });
                    }
                    else
                    {
                        cursor.Tree!.GetParent(out use);
                    }

                    // TODO: If there are side effects, need to preserve them
                    // by extracting side effects and combining with new use via CommaEntry

                    // Replace the use
                    use!.Set(newUse);

                    _logger.LogInformation("   After:");
                    _logger.LogInformation(dumper.Dump(cursor.Stmt));

                    _flowgraph.SetStatementSequence(cursor.Stmt);
                }

                // Remove intermediate IV stores
                if (IntermediateIVStores.Count > 0)
                {
                    _logger.LogInformation("   Deleting stores of intermediate IVs");
                    for (int i = 0; i < IntermediateIVStores.Count; i++)
                    {
                        CursorInfo cursor = IntermediateIVStores.Bottom(i);
                        LocalVariableCommon store = (LocalVariableCommon)cursor.Tree!;

                        // Cannot remove as this will break any later phases looking for SSA defs
                        store.Data = new Int32ConstantEntry(0);
                        _flowgraph.SetStatementSequence(cursor.Stmt);
                    }
                }

                return true;
            }

            private StackEntry RephraseIV(ScevAddRec iv, ScevAddRec sourceIV, StackEntry sourceTree)
            {
                Debug.Assert(iv.Start == sourceIV.Start);

                if (iv.Step == sourceIV.Step)
                {
                    return sourceTree;
                }

                int? ivStep = iv.Step.GetConstantValue();
                int? sourceIVStep = sourceIV.Step.GetConstantValue();

                Debug.Assert(ivStep is not null);
                Debug.Assert(sourceIVStep is not null);

                Debug.Assert(iv.Type == sourceIV.Type);

                int scale = ivStep.Value / sourceIVStep.Value;
                if (MathHelpers.IsPow2(scale))
                {
                    if (iv.Type == VarType.Ptr)
                    {
                        return new BinaryOperator(Operation.Lsh, false, sourceTree, new NativeIntConstantEntry((short)MathHelpers.GetLog2(scale)), iv.Type);
                    }
                    else
                    {
                        return new BinaryOperator(Operation.Lsh, false, sourceTree, new Int32ConstantEntry(MathHelpers.GetLog2(scale)), iv.Type);
                    }
                }
                else
                {
                    return new BinaryOperator(Operation.Mul, false, sourceTree, new Int32ConstantEntry(scale), iv.Type);
                }
            }

            private StackEntry NewTempStore(int tempNumber, StackEntry value)
            {
                StackEntry store = new StoreLocalVariableEntry(tempNumber, false, value);
                return store;
            }

            private Statement NewStmtFromTree(StackEntry tree)
            {
                return new Statement(tree);
            }

            private BasicBlock? FindUpdateInsertionPoint(Stack<CursorInfo> cursors, Statement? afterStmt = null)
            {
                BasicBlock? insertionPoint = null;
                foreach (FlowEdge backEdge in Loop.BackEdges)
                {
                    if (insertionPoint is null)
                    {
                        insertionPoint = backEdge.Source;
                    }
                    else
                    {
                        insertionPoint = FlowgraphDominatorTree.Intersects(insertionPoint, backEdge.Source);
                    }
                }

                while (insertionPoint is not null && Loop.ContainsBlock(insertionPoint) && Loop.MayExecuteBlockMultipleTimesPerIteration(insertionPoint))
                {
                    insertionPoint = insertionPoint.ImmediateDominator;
                }

                if (insertionPoint is null || !Loop.ContainsBlock(insertionPoint))
                {
                    return null;
                }

                if (!InsertionPointPostDominatesUses(insertionPoint, cursors))
                {
                    return null;
                }

                _logger.LogInformation("   Found a legal insertion point in {InsertionBasicBlockNumber}", insertionPoint.Label);

                return insertionPoint;
            }

            public bool InsertionPointPostDominatesUses(BasicBlock insertionPoint, Stack<CursorInfo> cursors)
            {
                for (int i = 0; i < cursors.Count; i++)
                {
                    CursorInfo cursor = cursors.Bottom(i);

                    if (insertionPoint == cursor.Block)
                    {
                        if (insertionPoint.HasTerminator && cursor.Stmt == insertionPoint.Statements[^1])
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (Loop.IsPostDominatedOnLoopIteration(cursor.Block, insertionPoint))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            private bool CheckAdvancedCursors(Stack<CursorInfo> cursors, out ScevAddRec? nextIV)
            {
                nextIV = null;
                bool allowRephrasingNextIV = true;

                for (int i = 0; i < cursors.Count; i++)
                {
                    CursorInfo cursor = cursors.Bottom(i);

                    if (cursor.IV is not null)
                    {
                        bool allowRephrasingViaScaling = true;

                        if (nextIV is null)
                        {
                            nextIV = cursor.IV;
                            allowRephrasingNextIV = allowRephrasingViaScaling;
                            continue;
                        }

                        ScevAddRec? rephrasableAddRec = ComputeRephrasableIV(cursor.IV, allowRephrasingViaScaling, nextIV, allowRephrasingNextIV);
                        if (rephrasableAddRec is not null)
                        {
                            nextIV = rephrasableAddRec;
                            allowRephrasingNextIV &= allowRephrasingViaScaling;
                            continue;
                        }
                    }

                    _logger.LogInformation("   {I} does not match; will not advance", i);
                    return false;
                }

                return nextIV is not null;
            }

            private ScevAddRec? ComputeRephrasableIV(ScevAddRec iv1, bool allowRephrasingViaScalingIV1, ScevAddRec iv2, bool allowRephrasingViaScalingIV2)
            {
                if (iv1.Start != iv2.Start)
                {
                    return null;
                }

                if (iv1.Step == iv2.Step)
                {
                    return iv1;
                }

                return ComputeRephrasableIVByScaling(iv1, allowRephrasingViaScalingIV1, iv2, allowRephrasingViaScalingIV2);
            }

            private ScevAddRec? ComputeRephrasableIVByScaling(ScevAddRec iv1, bool allowRephrasingViaScalingIV1, ScevAddRec iv2, bool allowRephrasingViaScalingIV2)
            {
                if (iv1.Start.GetConstantValue() != 0 || iv2.Start.GetConstantValue() != 0)
                {
                    return null;
                }

                int? iv1Step = iv1.Step.GetConstantValue();
                int? iv2Step = iv2.Step.GetConstantValue();

                if (iv1Step is null || iv2Step is null)
                {
                    return null;
                }

                int gcd = MathHelpers.Gcd(iv1Step.Value, iv2Step.Value);

                if ((!allowRephrasingViaScalingIV1 && gcd != iv1Step.Value) || (!allowRephrasingViaScalingIV2 && gcd != iv2Step.Value))
                {
                    return null;
                }

                if (gcd == iv1Step.Value)
                {
                    return iv1;
                }
                if (gcd == iv2Step.Value)
                {
                    return iv2;
                }

                if ((gcd == 1) || (gcd == -1))
                {
                    return null;
                }

                return ScalarEvolutionContext.NewAddRec(iv1.Start, ScalarEvolutionContext.NewConstant(iv1.Type, gcd));
            }

            private bool InitializeCursors(LocalVariableCommon primaryIVLcl, ScevAddRec primaryIV)
            {
                Cursors1.Clear();
                Cursors2.Clear();
                IntermediateIVStores.Clear();

                bool Visitor(BasicBlock block, Statement stmt, LocalVariableCommon tree)
                {
                    if (IsUseExpectedToBeRemoved(block, stmt, tree))
                    {
                        return true;
                    }

                    if (tree is not LocalVariableEntry)
                    {
                        return false;
                    }

                    if (tree.SsaNumber != primaryIVLcl.SsaNumber)
                    {
                        return false;
                    }

                    Scev? iv = ScalarEvolutionContext.Analyze(block, tree);
                    if (iv is null)
                    {
                        return false;
                    }

                    Debug.Assert(iv.Simplify(ScalarEvolutionContext).Equals(primaryIV));

                    Cursors1.Push(new CursorInfo(block, stmt, tree, primaryIV));
                    Cursors2.Push(new CursorInfo(block, stmt, tree, primaryIV));
                    return true;
                }

                if (!LoopInfo.VisitOccurrences(Loop, primaryIVLcl.LocalNumber, Visitor))
                {
                    _logger.LogInformation("   Could not create cursors for all loop uses of primary IV");
                    return false;
                }

                ExpandStoredCursors(Cursors1, Cursors2);

                _logger.LogInformation("   Found {CursorCount} cursors using primary IV V{LocalNumber}", Cursors1.Count, primaryIVLcl.LocalNumber);

                for (int i = 0; i < Cursors1.Count; i++)
                {
                    CursorInfo cursor = Cursors1.Bottom(i);
                    _logger.LogInformation("   [{I}] [{TreeID}]: {IV}", i, cursor.Tree!.TreeID, cursor.IV);
                }

                return true;
            }

            private static bool IsUseExpectedToBeRemoved(BasicBlock block, Statement stmt, LocalVariableCommon tree)
            {
                if (IsUpdateOfIVWithoutSideEffects(stmt.RootNode, tree.LocalNumber))
                {
                    return true;
                }

                // Check if use is inside exit test
                // TODO: Need to check if true/false target of conditional is outside the loop
                bool IsInsideExitTest = block.JumpKind == JumpKind.Conditional && stmt == block.Statements[^1];
                if (IsInsideExitTest)
                {
                    return true;
                }

                return false;
            }

            private static bool IsUpdateOfIVWithoutSideEffects(StackEntry tree, int localNumber)
            {
                if (tree is not StoreLocalVariableEntry)
                {
                    return false;
                }

                var store = (LocalVariableCommon)tree;
                if (store.LocalNumber != localNumber)
                {
                    return false;
                }

                // TODO: Check for side effects

                return true;
            }

            private void AdvanceCursors(Stack<CursorInfo> cursors, Stack<CursorInfo> nextCursors)
            {
                for (int i = 0; i < cursors.Count; i++)
                {
                    CursorInfo cursor = cursors.Bottom(i);
                    CursorInfo nextCursor = nextCursors.Bottom(i);

                    Debug.Assert(nextCursor.Block == cursor.Block && nextCursor.Stmt == cursor.Stmt);

                    nextCursor.Tree = cursor.Tree;
                    AdvanceCursor(cursor, nextCursor);
                }

                for (int i = 0; i < nextCursors.Count; i++)
                {
                    CursorInfo nextCursor = nextCursors.Bottom(i);
                    int treeId = nextCursor.Tree is null ? 0 : nextCursor.Tree.TreeID;
                    string ivDump = nextCursor.IV is null ? "<null IV>" : nextCursor.IV.ToString();
                    _logger.LogInformation("   [{I}] [{TreeId}]: {IVDump}", i, treeId, ivDump);
                }
            }

            private void AdvanceCursor(CursorInfo cursor, CursorInfo nextCursor)
            {
                do
                {
                    StackEntry? current = nextCursor.Tree;
                    nextCursor.Tree = current!.GetParent(out _);

                    if (nextCursor.Tree is null || nextCursor.Tree is CommaEntry commaEntry && commaEntry.Op1 == current)
                    {
                        nextCursor.IV = null;
                        break;
                    }

                    Scev? parentIV = ScalarEvolutionContext.Analyze(nextCursor.Block, nextCursor.Tree);
                    if (parentIV is null)
                    {
                        nextCursor.IV = null;
                        break;
                    }

                    parentIV = parentIV.Simplify(ScalarEvolutionContext);
                    Debug.Assert(parentIV is not null);
                    if (parentIV is not ScevAddRec)
                    {
                        nextCursor.IV = null;
                        break;
                    }

                    nextCursor.IV = (ScevAddRec)parentIV;
                }
                while (nextCursor.IV == cursor.IV);
            }
        }

        internal class CursorInfo(BasicBlock block, Statement stmt, StackEntry? tree, ScevAddRec? iv)
        {
            public BasicBlock Block = block;
            public Statement Stmt = stmt;
            public StackEntry? Tree = tree;
            public ScevAddRec? IV = iv;
        }
    }
}
