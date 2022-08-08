using ILCompiler.Common.TypeSystem;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler
{
    public class Morpher : IMorpher
    {
        public void Morph(IList<BasicBlock> blocks)
        {
            foreach (var block in blocks)
            {
                // fgMorphBlocks -> fgMorphStmts -> fgMorphTree -> fgMorphSmpOp -> fgMorphArrayIndex
                MorphStmts(block);
            }
        }

        private static void MorphStmts(BasicBlock block)
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

        private static StackEntry MorphTree(StackEntry tree)
        {
            switch (tree)
            {
                case CallEntry c:
                    tree = new CallEntry(c.TargetMethod, MorphList(c.Arguments), c.Kind, c.ExactSize);
                    break;

                case BinaryOperator bo:
                    tree = new BinaryOperator(bo.Operation, bo.IsComparison, MorphTree(bo.Op1), MorphTree(bo.Op2), bo.Kind);
                    break;

                case CastEntry ce:
                    tree = new CastEntry(ce.DesiredType, MorphTree(ce.Op1), ce.Kind);
                    break;

                case FieldAddressEntry fae:
                    tree = new FieldAddressEntry(fae.Name, MorphTree(fae.Op1), fae.Offset);
                    break;

                case IndirectEntry ie:
                    tree = new IndirectEntry(MorphTree(ie.Op1), ie.Kind, ie.ExactSize, ie.DesiredSize, ie.Offset) { SourceInHeap = ie.SourceInHeap };
                    break;

                case IntrinsicEntry ie:
                    tree = new IntrinsicEntry(ie.TargetMethod, MorphList(ie.Arguments), ie.Kind);
                    break;

                case JumpTrueEntry jte:
                    tree = new JumpTrueEntry(jte.TargetLabel, MorphTree(jte.Condition));
                    break;

                case LocalHeapEntry lhe:
                    tree = new LocalHeapEntry(MorphTree(lhe.Op1));
                    break;

                case ReturnEntry re:
                    tree = new ReturnEntry(re.Return, re.ReturnBufferArgIndex, re.ReturnTypeExactSize);
                    break;

                case StoreIndEntry sie:
                    tree = new StoreIndEntry(sie.Addr, MorphTree(sie.Op1), sie.TargetType, sie.FieldOffset, sie.ExactSize) { TargetInHeap = sie.TargetInHeap };
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
            }
            return tree;
        }

        private static IList<StackEntry> MorphList(IList<StackEntry> list)
        {
            var morphedItems = new List<StackEntry>();
            foreach (var item in list)
            {
                morphedItems.Add(MorphTree(item));
            }
            return morphedItems;
        }

        private static StackEntry MorphArrayIndex(IndexRefEntry tree)
        {
            StackEntry addr = tree.IndexOp;
            if (tree.ElemSize > 1)
            {
                // elemSize * index
                var size = new NativeIntConstantEntry((short)tree.ElemSize);
                var indexOp = new CastEntry(WellKnownType.UIntPtr, tree.IndexOp, StackValueKind.NativeInt);
                addr = new BinaryOperator(Operation.Mul, isComparison: false, size, indexOp, StackValueKind.NativeInt);
            }

            addr = new BinaryOperator(Operation.Add, isComparison: false, tree.ArrayOp, addr, StackValueKind.NativeInt);


            addr = new IndirectEntry(addr, tree.Kind, tree.ExactSize) { SourceInHeap = true };

            return addr;
        }
    }
}
