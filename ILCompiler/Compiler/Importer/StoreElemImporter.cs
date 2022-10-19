using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class StoreElemImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Stelem_I || code == Code.Stelem_I1 || code == Code.Stelem_I2 || code == Code.Stelem_I4 || code == Code.Stelem_Ref;

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var elemType = GetType(instruction.OpCode.Code);
            var elemSize = elemType.GetTypeSize();

            var value = importer.PopExpression();
            if (value.Type != elemType && elemType != VarType.Ref)
            {
                value = new CastEntry(value, elemType);
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

            addr = new BinaryOperator(Operation.Add, isComparison: false, arrayOp, addr, VarType.Ptr);

            var op = new StoreIndEntry(addr, value, 0, elemSize);
            importer.ImportAppendTree(op);
        }

        private static VarType GetType(Code code)
        {
            return code switch
            {
                Code.Stelem_I1 => VarType.SByte,
                Code.Stelem_I2 => VarType.Short,
                Code.Stelem_I4 => VarType.Int,
                Code.Stelem_I => VarType.Ptr,
                Code.Stelem_Ref => VarType.Ref,
                _ => throw new NotImplementedException(),
            };
        }

    }
}
