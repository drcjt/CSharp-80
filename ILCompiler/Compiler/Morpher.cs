using ILCompiler.Compiler.EvaluationStack;
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
        private int GrabTemp(VarType type, int? exactSize)
        {
            var temp = new LocalVariableDescriptor()
            {
                IsParameter = false,
                IsTemp = true,
                ExactSize = exactSize ?? 0,
                Type = type
            };

            _localVariableTable!.Add(temp);

            return _localVariableTable.Count - 1;
        }

        private StackEntry MorphArrayIndex(IndexRefEntry tree)
        {
            var arrayRef = tree.ArrayOp;
            var index = tree.IndexOp;
            StackEntry? boundsCheck = null;
            StackEntry? indexDefinition = null;
            StackEntry? arrayRefDefinition = null;

            if (tree.BoundsCheck)
            {
                // Allocate temporaries to store the array and index expressions
                // to ensure that if these expression involve assignments or calls
                // that the same values are used in the bounds check as the actual
                // array dereference.

                // TODO: Need to move GrabTemp to be in common place outside of the ILImporter
                // Really need to create new class to represent the LocalVariableTable and add GrabTemp as a method on this
                // The class will hold the list of local variables, but then need to change everything that is using
                // IList<LocalVariableDescriptor> to use the new LocalVariableTable type instead
                var arrayRefTemporaryNumber = GrabTemp(arrayRef.Type, arrayRef.ExactSize);
                arrayRefDefinition = new StoreLocalVariableEntry(arrayRefTemporaryNumber, false, arrayRef);
                arrayRef = new LocalVariableEntry(arrayRefTemporaryNumber, arrayRef.Type, arrayRef.ExactSize);
                var arrayRef2 = new LocalVariableEntry(arrayRefTemporaryNumber, arrayRef.Type, arrayRef.ExactSize);

                var indexTemporaryNumber = GrabTemp(index.Type, index.ExactSize);
                indexDefinition = new StoreLocalVariableEntry(indexTemporaryNumber, false, index);
                index = new LocalVariableEntry(indexTemporaryNumber, index.Type, index.ExactSize);
                var index2 = new LocalVariableEntry(indexTemporaryNumber, index.Type, index.ExactSize);

                // Create IR node to work out the array length
                var arraySizeOffset = new NativeIntConstantEntry(2);
                var arrayLengthPointer = new BinaryOperator(Operation.Add, isComparison: false, arrayRef, arraySizeOffset, VarType.Ptr);
                var arrayLength = new IndirectEntry(arrayLengthPointer, VarType.Ptr, 2);

                // Bounds check node taking the index and the array length
                boundsCheck = new BoundsCheck(new CastEntry(index, VarType.Ptr), arrayLength);

                arrayRef = arrayRef2;
                index = index2;
            }

            StackEntry addr = new CastEntry(index, VarType.Ptr);

            if (tree.ElemSize > 1)
            {
                // elemSize * index
                var size = new NativeIntConstantEntry((short)tree.ElemSize);
                addr = new BinaryOperator(Operation.Mul, isComparison: false, size, addr, VarType.Ptr);
            }

            // addr + arraySizeOffset + firstElemOffset + (elemSize * index)
            var offset = new NativeIntConstantEntry((short)(2 + tree.FirstElementOffset));
            addr = new BinaryOperator(Operation.Add, isComparison: false, addr, offset, VarType.Ptr);
            addr = new BinaryOperator(Operation.Add, isComparison: false, arrayRef, addr, VarType.Ptr);
            addr = new IndirectEntry(addr, tree.Type, tree.ElemSize);

            // Do bounds check first and then the array operation
            if (boundsCheck != null)
            {
                addr = new CommaEntry(boundsCheck, addr);
            }

            // Ensure any temporaries are defined before everything else
            if (indexDefinition != null)
            {
                addr = new CommaEntry(indexDefinition, addr);
            }
            if (arrayRefDefinition != null) 
            {
                addr = new CommaEntry(arrayRefDefinition, addr);
            }

            // Morph new tree in case any sub part of it includes stuff that needs to be morphed
            addr = MorphTree(addr);

            return addr;
        }
    }
}
