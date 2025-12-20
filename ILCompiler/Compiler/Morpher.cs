using System.Diagnostics;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.Helpers;
using ILCompiler.Compiler.OpcodeImporters;
using ILCompiler.Compiler.PreInit;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using StackEntry = ILCompiler.Compiler.EvaluationStack.StackEntry;

namespace ILCompiler.Compiler
{
    public class Morpher : IMorpher
    {
        private readonly PreinitializationManager _preinitializationManager;
        private readonly INameMangler _nameMangler;
        private readonly CodeFolder _codeFolder;

        private MethodCompiler _compiler = default!;

        public Morpher(PreinitializationManager preinitializationManager, INameMangler nameMangler, CodeFolder codeFolder)
        {
            _preinitializationManager = preinitializationManager;
            _nameMangler = nameMangler;
            _codeFolder = codeFolder;
        }

        private LocalVariableTable? _locals;
        public void Morph()
        {
            _locals = _compiler.Locals;
            foreach (var block in _compiler.Blocks)
            {
                // fgMorphBlocks -> fgMorphStmts -> fgMorphTree -> fgMorphSmpOp -> fgMorphArrayIndex
                MorphStatements(block);
            }
        }

        public void Init(MethodCompiler compiler)
        {
            _compiler = compiler;

            MethodDesc method = _compiler.Method!;

            var staticConstructorMethod = method.GetStaticConstructor();
            if (staticConstructorMethod is not null && !_preinitializationManager.IsPreinitialized(method.OwningType))
            {
                // Generate call to static constructor
                var targetMethod = _nameMangler.GetMangledMethodName(staticConstructorMethod);
                var staticInitCall = new CallEntry(targetMethod, [], VarType.Void, 0, method : staticConstructorMethod );

                var newStatement = new Statement(staticInitCall);

                if (_compiler.Blocks.Count > 0)
                {
                    var firstBlock = _compiler.Blocks[0];

                    if (firstBlock.Statements.Count > 0)
                    {
                        // Insert at the beginning of the first block
                        firstBlock.Statements.Insert(0, newStatement);
                    }
                    else
                    {
                        // If the first block is empty, just add the statement
                        firstBlock.Statements.Add(newStatement);
                    }
                }
            }
        }

        private void MorphStatements(BasicBlock block)
        {
            var morphedStatements = new List<Statement>();
            foreach (var statement in block.Statements)
            {
                if (statement.RootNode is not NothingEntry)
                {
                    morphedStatements.Add(MorphStatement(statement));
                }
            }

            block.Statements.Clear();
            foreach (var morphedStatement in morphedStatements)
            {
                block.Statements.Add(morphedStatement);
            }

            FoldConditional(block);
        }

        private static void FoldConditional(BasicBlock block)
        {
            if (block.Statements.Count == 0)
            {
                // Nothing to fold
                return;
            }

            if (block.JumpKind == JumpKind.Conditional)
            {
                var lastStatement = block.Statements[^1];

                Debug.Assert(lastStatement.RootNode is JumpTrueEntry);

                var jumpTrueEntry = (JumpTrueEntry)lastStatement.RootNode;
                if (jumpTrueEntry.Condition.IsIntCnsOrI())
                {
                    // The conditional can be folded away if the condition is a constant

                    // Remove the last statement from the block
                    block.Statements.RemoveAt(block.Statements.Count - 1);

                    var targetBlock = block.Successors.First(b => b.Label == jumpTrueEntry.TargetLabel);
                    var fallthroughBlock = block.Successors.First(b => b != targetBlock && !b.HandlerStart);

                    var constantValue = jumpTrueEntry.Condition.GetIntConstant();
                    if (constantValue == 0)
                    {
                        // Remove the target block
                        block.Successors.Remove(targetBlock);
                        targetBlock.Predecessors.Remove(block);
                    }
                    else
                    {
                        // Remove the fallthrough block
                        block.Successors.Remove(fallthroughBlock);
                        fallthroughBlock.Predecessors.Remove(block);
                    }

                    // The block is now unconditional
                    block.JumpKind = JumpKind.Always;
                }
            }
        }

        private Statement MorphStatement(Statement statement) => new Statement(MorphTree(statement.RootNode));

        private StackEntry MorphTree(StackEntry tree)
        {
            switch (tree)
            {
                case BoundsCheck be:
                    tree = new BoundsCheck(MorphTree(be.Index), MorphTree(be.ArrayLength));
                    break;

                case CommaEntry ce:
                    tree = new CommaEntry(MorphTree(ce.Op1), MorphTree(ce.Op2));
                    break;

                case CallEntry c:
                    tree = new CallEntry(c.TargetMethod, MorphList(c.Arguments), c.Type, c.ExactSize, c.IsVirtual, c.Method, c.IsInlineCandidate);
                    break;

                case BinaryOperator bo:
                    tree = MorphBinaryOperator(bo);
                    break;

                case CastEntry ce:
                    var cast = new CastEntry(MorphTree(ce.Op1), ce.Type);
                    tree = cast;                    
                    break;

                case FieldAddressEntry fae:
                    tree = new FieldAddressEntry(fae.Name, MorphTree(fae.Op1), fae.Offset);
                    break;

                case IndirectEntry ie:
                    tree = MorphIndirectEntry(ie);
                    break;

                case IntrinsicEntry ie:
                    tree = new IntrinsicEntry(ie.TargetMethod, MorphList(ie.Arguments), ie.Type);
                    break;

                case JumpTrueEntry jte:
                    {
                        if (jte.Condition is BinaryOperator binOp && binOp.IsComparison)
                        {
                            var morphedConditionTree = MorphBinaryOperator(binOp);
                            morphedConditionTree.ResultUsedInJump = true;
                            tree = new JumpTrueEntry(jte.TargetLabel, _codeFolder.FoldExpression(morphedConditionTree));
                    }
                    else
                        {
                            tree = new JumpTrueEntry(jte.TargetLabel, MorphTree(jte.Condition));
                        }
                    }
                    break;

                case LocalHeapEntry lhe:
                    tree = new LocalHeapEntry(MorphTree(lhe.Op1));
                    break;

                case ReturnEntry re:
                    var returnValue = re.Return;
                    if (returnValue != null)
                    {
                        returnValue = MorphTree(returnValue);
                    }
                    tree = new ReturnEntry(returnValue);
                    break;

                case StoreIndEntry sie:
                    tree = MorphStoreIndEntry(sie);
                    break;

                case StoreLocalVariableEntry slve:
                    tree = new StoreLocalVariableEntry(slve.LocalNumber, slve.IsParameter, MorphTree(slve.Op1));
                    break;

                case SwitchEntry se:
                    tree = new SwitchEntry(MorphTree(se.Op1), se.JumpTable);
                    break;

                case UnaryOperator uo:
                    tree = new UnaryOperator(uo.Operation, MorphTree(uo.Op1));
                    break;

                case IndexRefEntry indexRefEntry:
                    tree = MorphArrayIndex(indexRefEntry);
                    break;

                case PutArgTypeEntry putArgTypeEntry:
                    tree = new PutArgTypeEntry(putArgTypeEntry.ArgType, MorphTree(putArgTypeEntry.Op1));
                    break;

                case NullCheckEntry nullCheckEntry:
                    tree = new NullCheckEntry(MorphTree(nullCheckEntry.Op1));
                    break;

                case ArrayLengthEntry arrayLengthEntry:
                    tree = new ArrayLengthEntry(MorphTree(arrayLengthEntry.ArrayReference));
                    break;
            }
            return tree;
        }

        private IList<StackEntry> MorphList(IList<StackEntry> list)
        {
            var morphedItems = new List<StackEntry>();
            foreach (var item in list)
            {
                morphedItems.Add(MorphTree(item));
            }
            return morphedItems;
        }

        private StackEntry MorphArrayIndex(IndexRefEntry tree)
        {
            var arrayRef = tree.ArrayOp;
            var index = tree.IndexOp;

            var arrayElementHelper = new ArrayElementHelper(_locals!);
            var addr = arrayElementHelper.CreateArrayAccess(index, arrayRef, tree.Type, tree.ElemSize, false, tree.BoundsCheck, tree.FirstElementOffset);

            // Morph new tree in case any sub part of it includes stuff that needs to be morphed
            addr = MorphTree(addr);

            return addr;
        }

        private IndirectEntry MorphIndirectEntry(IndirectEntry tree)
        {
            var offset = tree.Offset;
            if (tree.Op1 is SymbolConstantEntry sce)
            {
                // Move offset into SymbolConstantEntry
                sce.Offset += (int)tree.Offset;
                offset = 0;
            }

            if (tree.Op1 is CommaEntry ce && ce.Op2 is SymbolConstantEntry sce2)
            {
                // Move offset into SymbolConstantEntry
                sce2.Offset += (int)tree.Offset;
                offset = 0;
            }

            return new IndirectEntry(MorphTree(tree.Op1), tree.Type, tree.ExactSize, offset);
        }

        private StoreIndEntry MorphStoreIndEntry(StoreIndEntry sie)
        {
            var fieldOffset = sie.FieldOffset;
            if (sie.Addr is SymbolConstantEntry sce)
            {
                // Move offset into SymbolConstantEntry
                sce.Offset += (int)fieldOffset;
                fieldOffset = 0;
            }

            if (sie.Op1 is CommaEntry ce && ce.Op2 is SymbolConstantEntry sce2)
            {
                // Move offset into SymbolConstantEntry
                sce2.Offset += (int)fieldOffset;
                fieldOffset = 0;
            }

            return new StoreIndEntry(MorphTree(sie.Addr), MorphTree(sie.Op1), sie.Type, fieldOffset, sie.ExactSize);
        }

        private BinaryOperator MorphBinaryOperator(BinaryOperator bo)
        {
            switch (bo.Operation)
            {
                case Operation.Sub:
                    return MorphSub(bo);

                case Operation.Lt:
                case Operation.Le:
                    return MorphLessThanOrEqual(bo);

                case Operation.Lt_Un:
                case Operation.Le_Un:
                    return MorphLessThanOrEqualUnsigned(bo);

                case Operation.Add:
                case Operation.Mul:
                case Operation.Or:
                case Operation.Xor:
                case Operation.And:
                    return OptimizeCommutativeArithmetic(bo);

                default:
                    return new BinaryOperator(bo.Operation, bo.IsComparison, MorphTree(bo.Op1), MorphTree(bo.Op2), bo.Type);
            }
        }

        private BinaryOperator MorphLessThanOrEqual(BinaryOperator bo)
        {
            // Convert Le and Lt into Ge and Gt
            var newOp = Operation.Ge + (bo.Operation - Operation.Le);
            return new BinaryOperator(newOp, bo.IsComparison, MorphTree(bo.Op2), MorphTree(bo.Op1), bo.Type);
        }

        private BinaryOperator MorphLessThanOrEqualUnsigned(BinaryOperator bo)
        {
            // Convert Le_Un and Lt_Un into Ge_Un and Gt_Un
            var newOp = Operation.Ge_Un + (bo.Operation - Operation.Le_Un);
            return new BinaryOperator(newOp, bo.IsComparison, MorphTree(bo.Op2), MorphTree(bo.Op1), bo.Type);
        }

        private BinaryOperator MorphSub(BinaryOperator bo)
        {
            // Convert "op1-constant" into addition e.g. "op1 + (-constant)@
            if (bo.Op2.IsIntCnsOrI())
            {
                var newTree = new BinaryOperator(Operation.Add, bo.IsComparison, MorphTree(bo.Op1), bo.Op2.NegateIntCnsOrI(), bo.Type);
                return MorphBinaryOperator(newTree);
            }

            return new BinaryOperator(bo.Operation, bo.IsComparison, MorphTree(bo.Op1), MorphTree(bo.Op2), bo.Type);
        }

        private BinaryOperator OptimizeCommutativeArithmetic(BinaryOperator bo)
        {
            // Commute constants to the right
            if (bo.Op1.IsIntCnsOrI())
            {
                return new BinaryOperator(bo.Operation, bo.IsComparison, MorphTree(bo.Op2), MorphTree(bo.Op1), bo.Type);
            }

            return MorphCommutative(bo);
        }

        /// <summary>
        /// Try to simplify "(X op Constant1) op Constant2" to "X op Cosntant3"
        /// </summary>
        /// <param name="bo"></param>
        /// <returns></returns>
        private BinaryOperator MorphCommutative(BinaryOperator bo)
        {
            if (bo.Op1 is BinaryOperator boLeftChild && 
                boLeftChild.Op2.IsIntCnsOrI() && bo.Op2.IsIntCnsOrI() && 
                boLeftChild.Operation == bo.Operation)
            {
                var newOperNode = new BinaryOperator(bo.Operation, bo.IsComparison, boLeftChild.Op2, bo.Op2, bo.Type);
                var foldedNewOperNode = _codeFolder.FoldConstantExpression(newOperNode);

                return new BinaryOperator(bo.Operation, bo.IsComparison, MorphTree(boLeftChild.Op1), foldedNewOperNode, bo.Type);
            }

            return new BinaryOperator(bo.Operation, bo.IsComparison, MorphTree(bo.Op1), MorphTree(bo.Op2), bo.Type);
        }
    }
}
