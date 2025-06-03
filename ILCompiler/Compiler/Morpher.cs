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
        public Morpher(PreinitializationManager preinitializationManager, INameMangler nameMangler)
        {
            _preinitializationManager = preinitializationManager;
            _nameMangler = nameMangler;
        }

        private LocalVariableTable? _locals;
        public void Morph(IList<BasicBlock> blocks, LocalVariableTable locals)
        {
            _locals = locals;
            foreach (var block in blocks)
            {
                // fgMorphBlocks -> fgMorphStmts -> fgMorphTree -> fgMorphSmpOp -> fgMorphArrayIndex
                MorphStatements(block);
            }
        }

        public void Init(MethodDesc method, IList<BasicBlock> blocks)
        {
            if (method.IsStatic && !_preinitializationManager.IsPreinitialized(method.OwningType))
            {
                var staticConstructorMethod = method.OwningType.GetStaticConstructor();
                if (staticConstructorMethod != null && staticConstructorMethod.FullName != method.FullName)
                {
                    // Generate call to static constructor
                    var targetMethod = _nameMangler.GetMangledMethodName(staticConstructorMethod);
                    var staticInitCall = new CallEntry(targetMethod, [], VarType.Void, 0);

                    var newStatement = new Statement(staticInitCall);

                    if (blocks.Count > 0)
                    {
                        var firstBlock = blocks[0];

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
        }

        private void MorphStatements(BasicBlock block)
        {
            var morphedStatements = new List<Statement>();
            foreach (var statement in block.Statements)
            {
                morphedStatements.Add(MorphStatement(statement));
            }

            block.Statements.Clear();
            foreach (var morphedStatement in morphedStatements)
            {
                block.Statements.Add(morphedStatement);
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
                            tree = new JumpTrueEntry(jte.TargetLabel, morphedConditionTree);
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
                    tree = new ReturnEntry(returnValue, re.ReturnBufferArgIndex, re.ReturnTypeExactSize);
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
                var foldedNewOperNode = CodeFolder.FoldConstantExpression(newOperNode);

                return new BinaryOperator(bo.Operation, bo.IsComparison, MorphTree(boLeftChild.Op1), foldedNewOperNode, bo.Type);
            }

            return new BinaryOperator(bo.Operation, bo.IsComparison, MorphTree(bo.Op1), MorphTree(bo.Op2), bo.Type);
        }
    }
}
