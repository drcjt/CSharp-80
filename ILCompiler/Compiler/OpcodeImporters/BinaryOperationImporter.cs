using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class BinaryOperationImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
        {
            Operation binaryOp;
            switch (instruction.Opcode)
            {
                case ILOpcode.add:
                case ILOpcode.sub:
                case ILOpcode.mul:
                case ILOpcode.div:
                case ILOpcode.rem:
                case ILOpcode.div_un:
                case ILOpcode.rem_un:
                case ILOpcode.and:
                case ILOpcode.or:
                case ILOpcode.xor:
                    binaryOp = Operation.Add + (instruction.Opcode - ILOpcode.add);
                    break;
                case ILOpcode.mul_ovf_un:
                    // For now this maps to standard multiplication as we have no exception support
                    binaryOp = Operation.Mul;
                    break;

                default:
                    return false;
            }

            var op2 = importer.Pop();
            var op1 = importer.Pop();

            // If one of the values is a native int then cast the other to be native int too
            if (op1.Type == VarType.Ptr || op2.Type == VarType.Ptr)
            {
                if (op1.Type != VarType.Ptr)
                {
                    var cast = new CastEntry(op1, VarType.Ptr);
                    op1 = cast;
                }
                if (op2.Type != VarType.Ptr)
                {
                    var cast = new CastEntry(op2, VarType.Ptr);
                    op2 = cast;
                }
            }

            op1 = importer.CodeFolder.FoldExpression(op1);
            op2 = importer.CodeFolder.FoldExpression(op2);

            // Handle special case of int+0, int-0, int*1, int/1
            // All of these just turn into int
            if ((op2.IsIntegralConstant(0) && (binaryOp == Operation.Add || binaryOp == Operation.Sub)) ||
                op2.IsIntegralConstant(1) && (binaryOp == Operation.Mul || binaryOp == Operation.Div))
            {
                // If the second operand is a native int then need to still ensure op1 is cast to be a native int
                if (op2.Type == VarType.Ptr)
                {
                    op1 = importer.CodeFolder.FoldExpression(new CastEntry(op1, VarType.Ptr));
                }

                importer.Push(op1);
                return true;
            }

            var binaryExpr = new BinaryOperator(binaryOp, isComparison: false, op1, op2, GetResultType(op1, op2));
            importer.Push(binaryExpr);

            return true;
        }

        private static VarType GetResultType(StackEntry op1, StackEntry op2)
        {
            if (op1.Type == VarType.Ptr || op2.Type == VarType.Ptr)
            {
                return VarType.Ptr;
            }
            else if (op1.Type == VarType.ByRef && op2.Type == VarType.ByRef)
            {
                return VarType.ByRef;
            }

            return VarType.Int;
        }
    }
}
