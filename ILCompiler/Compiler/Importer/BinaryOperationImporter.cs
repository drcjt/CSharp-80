using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class BinaryOperationImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            Operation binaryOp;
            switch (instruction.OpCode.Code)
            {
                case Code.Add:
                case Code.Sub:
                case Code.Mul:
                case Code.Div:
                case Code.Rem:
                case Code.Div_Un:
                case Code.Rem_Un:
                case Code.And:
                case Code.Or:
                    binaryOp = Operation.Add + (instruction.OpCode.Code - Code.Add);
                    break;
                case Code.Mul_Ovf_Un:
                    // For now this maps to standard multiplication as we have no exception support
                    binaryOp = Operation.Mul;
                    break;

                default:
                    return false;
            }

            var op2 = importer.PopExpression();
            var op1 = importer.PopExpression();

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

            var binaryExpr = new BinaryOperator(binaryOp, isComparison: false, op1, op2, GetResultType(op1, op2));
            importer.PushExpression(binaryExpr);

            return true;
        }

        private static VarType GetResultType(StackEntry op1, StackEntry op2)
        {
            if (op1.Type == VarType.Ptr || op2.Type == VarType.Ptr)
            {
                return VarType.Ptr;
            }
            else
            {
                return VarType.Int;
            }
        }
    }
}
