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

            // TODO: Ideally want to compare based on stackvaluekind and not implementation sizes here
            if (TypeList.GetExactSize(op1.Kind) != TypeList.GetExactSize(op2.Kind))
            {
                throw new NotSupportedException($"Boolean comparisons must have same size, {op1.Kind} and {op2.Kind} have different sizes");
            }

            Operation op = Operation.Eq;
            switch (instruction.OpCode.Code)
            {
                case Code.Cgt: op = Operation.Gt; break;
                case Code.Cgt_Un: op = Operation.Gt; break;
                case Code.Clt: op = Operation.Lt; break;
                case Code.Clt_Un: op = Operation.Lt; break;
            }
            op1 = new BinaryOperator(op, isComparison: true, op1, op2, op1.Kind);
            importer.PushExpression(op1);
        }
    }
}
