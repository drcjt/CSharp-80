using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Helpers
{
    internal class ArrayElementHelper
    {
        private readonly LocalVariableTable _locals;

        public ArrayElementHelper(LocalVariableTable locals)
        {
            _locals = locals;
        }

        private int GrabTemp(IImporter? importer, VarType type, int? exactSize)
        {
            if (importer is not null)
                return importer.GrabTemp(type, exactSize);
            else
                return _locals.GrabTemp(type, exactSize);
        }

        public StackEntry CreateArrayAccess(StackEntry index, StackEntry arrayRef, VarType elemType, int elemSize, bool address, bool checkBounds, int firstElementOffset, IImporter? importer = null)
        {
            StackEntry? boundsCheck = null;
            StackEntry? indexDefinition = null;
            StackEntry? arrayRefDefinition = null;

            if (checkBounds)
            {
                // Allocate temporaries to store the array and index expressions
                // to ensure that if these expression involve assignments or calls
                // that the same values are used in the bounds check as the actual
                // array dereference.

                var arrayRefTemporaryNumber = GrabTemp(importer, arrayRef.Type, arrayRef.ExactSize);
                arrayRefDefinition = new StoreLocalVariableEntry(arrayRefTemporaryNumber, false, arrayRef);
                arrayRef = new LocalVariableEntry(arrayRefTemporaryNumber, arrayRef.Type, arrayRef.ExactSize);
                var arrayRef2 = new LocalVariableEntry(arrayRefTemporaryNumber, arrayRef.Type, arrayRef.ExactSize);

                var indexTemporaryNumber = GrabTemp(importer, index.Type, index.ExactSize);
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
            if (elemSize > 1)
            {
                // elemSize * index
                var size = new NativeIntConstantEntry((short)elemSize);
                addr = new BinaryOperator(Operation.Mul, isComparison: false, size, addr, VarType.Ptr);
            }

            // addr + arraySizeOffset + firstElemOffset + (elemSize * index)
            var offset = new NativeIntConstantEntry((short)(2 + firstElementOffset));
            addr = new BinaryOperator(Operation.Add, isComparison: false, addr, offset, VarType.Ptr);
            addr = new BinaryOperator(Operation.Add, isComparison: false, arrayRef, addr, VarType.Ptr);

            if (!address)
            {
                addr = new IndirectEntry(addr, elemType, elemSize);
            }

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

            return addr;
        }
    }
}
