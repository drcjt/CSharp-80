using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    internal class LoadElemAddressImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.OpCode.Code != Code.Ldelema) return false;

            var typeSig = (instruction.Operand as ITypeDefOrRef).ToTypeSig();
            typeSig = context.Method.ResolveType(typeSig);
            int elemSize = typeSig.GetInstanceFieldSize();

            // TODO
            // Consider using a helper function instead
            // ryujit uses a helper function here CORINFO_HELP_LDELEMA_REF

            var index = importer.PopExpression();
            var arrayRef = importer.PopExpression();

            StackEntry? boundsCheck = null;
            StackEntry? indexDefinition = null;
            StackEntry? arrayRefDefinition = null;

            if (!context.Configuration.SkipArrayBoundsCheck)
            {
                var arrayRefTemporaryNumber = GrabTemp(arrayRef.Type, arrayRef.ExactSize, importer.LocalVariableTable);
                arrayRefDefinition = new StoreLocalVariableEntry(arrayRefTemporaryNumber, false, arrayRef);
                arrayRef = new LocalVariableEntry(arrayRefTemporaryNumber, arrayRef.Type, arrayRef.ExactSize);
                var arrayRef2 = new LocalVariableEntry(arrayRefTemporaryNumber, arrayRef.Type, arrayRef.ExactSize);

                var indexTemporaryNumber = GrabTemp(index.Type, index.ExactSize, importer.LocalVariableTable);
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

            const int FirstElementOffset = 2;
            // addr + arraySizeOffset + firstElemOffset + (elemSize * index)
            var offset = new NativeIntConstantEntry((short)(2 + FirstElementOffset));
            addr = new BinaryOperator(Operation.Add, isComparison: false, addr, offset, VarType.Ptr);
            addr = new BinaryOperator(Operation.Add, isComparison: false, arrayRef, addr, VarType.Ptr);

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

            importer.PushExpression(addr);

            return true;
        }

        private static int GrabTemp(VarType type, int? exactSize, IList<LocalVariableDescriptor> localVariableTable)
        {
            var temp = new LocalVariableDescriptor()
            {
                IsParameter = false,
                IsTemp = true,
                ExactSize = exactSize ?? 0,
                Type = type
            };

            localVariableTable.Add(temp);

            return localVariableTable.Count - 1;
        }
    }
}
