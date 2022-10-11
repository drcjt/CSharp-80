using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class CompareImporter : IOpcodeImporter
    {
        public bool CanImport(Code code)
        {
            return code == Code.Ceq ||
                   code == Code.Cgt ||
                   code == Code.Cgt_Un ||
                   code == Code.Clt ||
                   code == Code.Cgt_Un;
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

            // TODO: Validate type of op1/op2 are compatible here

            Operation op = Operation.Eq;
            switch (instruction.OpCode.Code)
            {
                case Code.Cgt: op = Operation.Gt; break;
                case Code.Cgt_Un: op = Operation.Gt; break;
                case Code.Clt: op = Operation.Lt; break;
                case Code.Clt_Un: op = Operation.Lt; break;
            }
            op1 = new BinaryOperator(op, isComparison: true, op1, op2, VarType.Int);
            importer.PushExpression(op1);
        }
    }
}
