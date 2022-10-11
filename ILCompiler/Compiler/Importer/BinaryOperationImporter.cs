using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

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
            if (op1.Type == VarType.Ptr || op2.Type == VarType.Ptr)
            {
                if (op1.Type != VarType.Ptr)
                {
                    var cast = new CastEntry(Common.TypeSystem.WellKnownType.Object, op1, VarType.Ptr);
                    cast.DesiredType2 = VarType.Ptr;
                    op1 = cast;
                }
                if (op2.Type != VarType.Ptr)
                {
                    var cast = new CastEntry(Common.TypeSystem.WellKnownType.Object, op2, VarType.Ptr);
                    cast.DesiredType2 = VarType.Ptr;
                    op2 = cast;
                }
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

            var binaryExpr = new BinaryOperator(binaryOp, isComparison: false, op1, op2, GetResultType(op1, op2));
            importer.PushExpression(binaryExpr);
        }

        private VarType GetResultType(StackEntry op1, StackEntry op2)
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
