using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class StoreElemImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            VarType elemType;
            int elemSize = 0;
            switch (instruction.OpCode.Code)
            {
                case Code.Stelem:
                    var typeSig = (instruction.Operand as ITypeDefOrRef).ToTypeSig();
                    typeSig = context.Method.ResolveType(typeSig);
                    elemType = typeSig.GetVarType();
                    elemSize = typeSig.GetExactSize();
                    break;
                case Code.Stelem_I:
                    elemType = VarType.Ptr;
                    break;
                case Code.Stelem_I1:
                    elemType = VarType.Byte;
                    break;
                case Code.Stelem_I2:
                    elemType = VarType.Short;
                    break;
                case Code.Stelem_I4:
                    elemType = VarType.Int;
                    break;
                case Code.Stelem_Ref:
                    elemType = VarType.Ref;
                    break;

                default:
                    return false;
            }
            var value = importer.PopExpression();

            if (instruction.OpCode.Code != Code.Stelem) 
            {
                elemSize = elemType.GetTypeSize();

                if (value.Type != elemType && elemType != VarType.Ref)
                {
                    value = new CastEntry(value, elemType);
                }
            }

            var indexOp = importer.PopExpression();
            var arrayOp = importer.PopExpression();

            var cast = new CastEntry(indexOp, VarType.Ptr);
            StackEntry addr = cast;
            if (elemSize > 1)
            {
                // elemSize * index
                var size = new NativeIntConstantEntry((short)elemSize);
                addr = new BinaryOperator(Operation.Mul, isComparison: false, size, addr, VarType.Ptr);
            }

            // addr + arraySizeOffset + (elemSize * index)
            var arraySizeOffset = new NativeIntConstantEntry(2);
            addr = new BinaryOperator(Operation.Add, isComparison: false, addr, arraySizeOffset, VarType.Ptr);
            addr = new BinaryOperator(Operation.Add, isComparison: false, arrayOp, addr, VarType.Ptr);

            var op = new StoreIndEntry(addr, value, 0, elemSize);
            importer.ImportAppendTree(op);

            return true;
        }
    }
}
