using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class ShiftOperationImporter : IOpcodeImporter
    {
        public bool CanImport(Code opcode)
        {
            return opcode == Code.Shl || opcode == Code.Shr;
        }

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var op2 = importer.PopExpression();
            var op1 = importer.PopExpression(); // operand to be shifted

            if (op1.Kind != StackValueKind.Int32 && op1.Kind != StackValueKind.NativeInt)
            {
                throw new NotSupportedException($"Shift operation on type {op1.Kind} not supported");
            }

            if (op2.Kind != StackValueKind.Int32 && op2.Kind != StackValueKind.NativeInt)
            {
                throw new NotSupportedException($"Shift operation with amount of type {op2.Kind} not supported");
            }

            var shiftOp = Operation.Lsh + (instruction.OpCode.Code - Code.Shl);
            var binaryExpr = new BinaryOperator(shiftOp, isComparison: false, op1, op2, op1.Kind);
            importer.PushExpression(binaryExpr);
        }
    }
}
