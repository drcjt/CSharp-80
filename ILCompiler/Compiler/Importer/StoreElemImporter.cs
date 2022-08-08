using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class StoreElemImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Stelem_I4;

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var value = importer.PopExpression();
            var indexOp = importer.PopExpression();
            var arrayOp = importer.PopExpression();

            var elemSize = 4;

            StackEntry addr = indexOp;
            if (elemSize > 1)
            {
                // elemSize * index
                var size = new NativeIntConstantEntry((short)elemSize);
                indexOp = new CastEntry(WellKnownType.UIntPtr, indexOp, StackValueKind.NativeInt);
                addr = new BinaryOperator(Operation.Mul, isComparison: false, size, indexOp, StackValueKind.NativeInt);
            }

            addr = new BinaryOperator(Operation.Add, isComparison: false, arrayOp, addr, StackValueKind.NativeInt);

            var targetType = WellKnownType.Int32;

            var op = new StoreIndEntry(addr, value, targetType) { TargetInHeap = true };
            importer.ImportAppendTree(op);
        }
    }
}
