using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System;

namespace ILCompiler.Compiler.Importer
{
    public class CompareImporter : IOpcodeImporter
    {
        private readonly IILImporter _importer;

        public CompareImporter(IILImporter importer)
        {
            _importer = importer;
        }

        public bool CanImport(Code opcode)
        {
            return opcode == Code.Ceq;
        }

        public void Import(Instruction instruction, ImportContext context)
        {
            var op2 = _importer.PopExpression();
            if (op2.Kind != StackValueKind.Int32)
            {
                throw new NotSupportedException("Boolean comparisons only supported using int as underlying type");
            }
            StackEntry op1;
            var opcode = Code.Beq;
            var op = Operation.Eq + (opcode - Code.Beq);
            op1 = _importer.PopExpression();
            if (op2.Kind != StackValueKind.Int32)
            {
                throw new NotSupportedException("Boolean comparisons only supported using int as underlying type");
            }
            op1 = new BinaryOperator(op, op1, op2, StackValueKind.Int32);
            _importer.PushExpression(op1);
        }
    }
}
