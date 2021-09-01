using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System;

namespace ILCompiler.Compiler.Importer
{
    public class CompareImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Ceq;

        public void Import(Instruction instruction, ImportContext context, IILImporter importer)
        {
            var op2 = importer.PopExpression();
            if (op2.Kind != StackValueKind.Int32)
            {
                throw new NotSupportedException("Boolean comparisons only supported using int as underlying type");
            }
            StackEntry op1;
            var code = Code.Beq;
            var op = Operation.Eq + (code - Code.Beq);
            op1 = importer.PopExpression();
            if (op2.Kind != StackValueKind.Int32)
            {
                throw new NotSupportedException("Boolean comparisons only supported using int as underlying type");
            }
            op1 = new BinaryOperator(op, op1, op2, StackValueKind.Int32);
            importer.PushExpression(op1);
        }
    }
}
