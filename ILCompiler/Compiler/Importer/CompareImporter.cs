using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System;

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

        public void Import(Instruction instruction, ImportContext context, IILImporter importer)
        {
            var op2 = importer.PopExpression();
            if (op2.Kind != StackValueKind.Int32)
            {
                throw new NotSupportedException("Boolean comparisons only supported using int as underlying type");
            }
            StackEntry op1;
            Operation op = Operation.Eq;
            switch (instruction.OpCode.Code)
            {
                case Code.Cgt: op = Operation.Gt; break;
                case Code.Cgt_Un: op = Operation.Gt; break;
                case Code.Clt: op = Operation.Lt; break;
                case Code.Clt_Un: op = Operation.Lt; break;
            }
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
