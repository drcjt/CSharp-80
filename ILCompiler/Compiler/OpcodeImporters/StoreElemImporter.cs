using ILCompiler.TypeSystem.Common;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class StoreElemImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
        {
            VarType elemType;
            int elemSize = 0;
            switch (instruction.Opcode)
            {
                case ILOpcode.stelem:
                    var typeDesc = (TypeDesc)instruction.Operand;
                    elemType = typeDesc.VarType;
                    elemSize = typeDesc.GetElementSize().AsInt;
                    break;
                case ILOpcode.stelem_i:
                    elemType = VarType.Ptr;
                    break;
                case ILOpcode.stelem_i1:
                    elemType = VarType.Byte;
                    break;
                case ILOpcode.stelem_i2:
                    elemType = VarType.Short;
                    break;
                case ILOpcode.stelem_i4:
                    elemType = VarType.Int;
                    break;
                case ILOpcode.stelem_ref:
                    elemType = VarType.Ref;
                    break;

                default:
                    return false;
            }
            var value = importer.Pop();

            if (instruction.Opcode != ILOpcode.stelem) 
            {
                elemSize = elemType.GetTypeSize();

                if (value.Type != elemType && elemType != VarType.Ref)
                {
                    value = CodeFolder.FoldExpression(new CastEntry(value, elemType));
                }
            }

            var indexOp = importer.Pop();
            var arrayOp = importer.Pop();

            var cast = CodeFolder.FoldExpression(new CastEntry(indexOp, VarType.Ptr));
            StackEntry addr = cast;
            if (elemSize > 1)
            {
                // elemSize * index
                var size = new NativeIntConstantEntry((short)elemSize);
                addr = new BinaryOperator(Operation.Mul, isComparison: false, size, addr, VarType.Ptr);
            }

            // addr + arraySizeOffset + (elemSize * index)
            var arraySizeOffset = new NativeIntConstantEntry(4);
            addr = new BinaryOperator(Operation.Add, isComparison: false, addr, arraySizeOffset, VarType.Ptr);
            addr = new BinaryOperator(Operation.Add, isComparison: false, arrayOp, addr, VarType.Ptr);

            StackEntry op = new StoreIndEntry(addr, value, elemType, 0, elemSize);

            if (elemType == VarType.Struct)
            {
                op = importer.StoreStruct(op);
            }

            importer.ImportAppendTree(op);

            return true;
        }
    }
}