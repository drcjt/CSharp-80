using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System.Diagnostics;

namespace ILCompiler.Compiler.Importer
{
    public class BinaryOperationImporter : IOpcodeImporter
    {
        public bool CanImport(Code opcode)
        {
            return opcode == Code.Add ||
                   opcode == Code.Sub ||
                   opcode == Code.Mul ||
                   opcode == Code.Div ||
                   opcode == Code.Rem ||
                   opcode == Code.Div_Un ||
                   opcode == Code.Rem_Un ||
                   opcode == Code.And ||
                   opcode == Code.Or ||
                   opcode == Code.Mul_Ovf_Un;
        }

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var op2 = importer.PopExpression();
            var op1 = importer.PopExpression();

            // If one of the values is a native int then cast the other to be native int too
            if (op1.Kind == StackValueKind.NativeInt && op2.Kind == StackValueKind.Int32)
            {
                op2 = new CastEntry(Common.TypeSystem.WellKnownType.Object, op2, op1.Kind);
            }
            else if (op1.Kind == StackValueKind.Int32 && op2.Kind == StackValueKind.NativeInt)
            {
                op1 = new CastEntry(Common.TypeSystem.WellKnownType.Object, op1, op2.Kind);
            }

            // StackValueKind is carefully ordered to make this work
            StackValueKind kind;
            kind = op1.Kind > op2.Kind ? op1.Kind : op2.Kind;

            if (kind != StackValueKind.Int32 && kind != StackValueKind.NativeInt)
            {
                throw new NotSupportedException($"Binary operation on type {kind} not supported");
            }

            Operation binaryOp;
            switch (instruction.OpCode.Code)
            {
                case Code.Mul_Ovf_Un:
                    // For now this maps to standard multiplication as we have no exception support
                    binaryOp = Operation.Mul;
                    break;

                default:
                    binaryOp = Operation.Add + (instruction.OpCode.Code - Code.Add);
                    break;
            }

            var binaryExpr = new BinaryOperator(binaryOp, isComparison: false, op1, op2, kind);
            importer.PushExpression(binaryExpr);
        }
    }
}
