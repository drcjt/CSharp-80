﻿using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.Helpers;
using ILCompiler.Compiler.Importer;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler
{
    public class Morpher : IMorpher
    {
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

        private void MorphStatements(BasicBlock block)
        {
            var morphedStatements = new List<StackEntry>();
            foreach (var statement in block.Statements)
            {
                morphedStatements.Add(MorphTree(statement));
            }

            block.Statements.Clear();
            foreach (var morphedStatement in morphedStatements)
            {
                block.Statements.Add(morphedStatement);
            }
        }

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
                    tree = new CallEntry(c.TargetMethod, MorphList(c.Arguments), c.Type, c.ExactSize, c.IsVirtual, c.Method);
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
                    tree = new IndirectEntry(MorphTree(ie.Op1), ie.Type, ie.ExactSize, ie.Offset);
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
                    tree = new StoreIndEntry(MorphTree(sie.Addr), MorphTree(sie.Op1), sie.Type, sie.FieldOffset, sie.ExactSize);
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

        private BinaryOperator MorphBinaryOperator(BinaryOperator bo)
        {
            switch (bo.Operation)
            {
                case Operation.Sub:
                    return MorphSub(bo);

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
            if (bo.Op1 is BinaryOperator boLeftChild)
            {
                if (boLeftChild.Op2.IsIntCnsOrI() && bo.Op2.IsIntCnsOrI() && boLeftChild.Operation == bo.Operation)
                {
                    var newOperNode = new BinaryOperator(bo.Operation, bo.IsComparison, boLeftChild.Op2, bo.Op2, bo.Type);
                    var foldedNewOperNode = CodeFolder.FoldConstantExpression(newOperNode);

                    return new BinaryOperator(bo.Operation, bo.IsComparison, MorphTree(boLeftChild.Op1), foldedNewOperNode, bo.Type);
                }
            }

            return new BinaryOperator(bo.Operation, bo.IsComparison, MorphTree(bo.Op1), MorphTree(bo.Op2), bo.Type);
        }
    }
}
