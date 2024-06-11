using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.Importer
{
    public class CompareImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            Operation op;
            switch (instruction.Opcode)
            {
                case ILOpcode.ceq:
                    op = Operation.Eq;
                    break;
                case ILOpcode.clt:
                    op = Operation.Lt;
                    break;
                case ILOpcode.clt_un:
                    op = Operation.Lt_Un;
                    break;
                case ILOpcode.cgt:
                    op = Operation.Gt;
                    break;
                case ILOpcode.cgt_un:
                    op = Operation.Gt_Un;
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
                    var cast = CodeFolder.FoldExpression(new CastEntry(op1, VarType.Ptr));
                    op1 = cast;
                }
                if (op2.Type != VarType.Ptr)
                {
                    var cast = CodeFolder.FoldExpression(new CastEntry(op2, VarType.Ptr));
                    op2 = cast;
                }
            }

            // TODO: Validate type of op1/op2 are compatible here

            op1 = new BinaryOperator(op, isComparison: true, op1, op2, VarType.Int);
            importer.PushExpression(op1);

            return true;
        }
    }
}
