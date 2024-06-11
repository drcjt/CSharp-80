using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.Importer
{
    public class ShiftOperationImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            Operation shiftOp;
            switch (instruction.Opcode)
            {
                case ILOpcode.shl:
                case ILOpcode.shr:
                    shiftOp = Operation.Lsh + (instruction.Opcode - ILOpcode.shl);
                    break;

                default:
                    return false;
            }
            var op2 = importer.PopExpression();
            var op1 = importer.PopExpression(); // operand to be shifted

            var binaryExpr = new BinaryOperator(shiftOp, isComparison: false, op1, op2, VarType.Int);
            importer.PushExpression(binaryExpr);

            return true;
        }
    }
}