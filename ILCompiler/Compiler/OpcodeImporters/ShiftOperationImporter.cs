using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class ShiftOperationImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IImporter importer)
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
            var op2 = importer.Pop();
            var op1 = importer.Pop(); // operand to be shifted

            var binaryExpr = new BinaryOperator(shiftOp, isComparison: false, op1, op2, VarType.Int);
            importer.Push(binaryExpr);

            return true;
        }
    }
}