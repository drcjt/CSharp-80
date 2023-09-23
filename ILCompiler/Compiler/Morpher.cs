using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.Helpers;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler
{
    public class Morpher : IMorpher
    {
        private IList<LocalVariableDescriptor>? _localVariableTable;
        public void Morph(IList<BasicBlock> blocks, IList<LocalVariableDescriptor> localVariableTable)
        {
            _localVariableTable = localVariableTable;
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
                    tree = new CallEntry(c.TargetMethod, MorphList(c.Arguments), c.Type, c.ExactSize, c.IsInternalCall);
                    break;

                case BinaryOperator bo:
                    tree = new BinaryOperator(bo.Operation, bo.IsComparison, MorphTree(bo.Op1), MorphTree(bo.Op2), bo.Type);
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
                    tree = new JumpTrueEntry(jte.TargetLabel, MorphTree(jte.Condition));
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

            var arrayElementHelper = new ArrayElementHelper(_localVariableTable!);
            var addr = arrayElementHelper.CreateArrayAccess(index, arrayRef, tree.Type, tree.ElemSize, false, tree.BoundsCheck, tree.FirstElementOffset);

            // Morph new tree in case any sub part of it includes stuff that needs to be morphed
            addr = MorphTree(addr);

            return addr;
        }
    }
}
