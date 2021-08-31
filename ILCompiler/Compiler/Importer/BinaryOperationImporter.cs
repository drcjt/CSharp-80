using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System;

namespace ILCompiler.Compiler.Importer
{
    public class BinaryOperationImporter : IOpcodeImporter
    {
        private readonly IILImporter _importer;
        public BinaryOperationImporter(IILImporter importer)
        {
            _importer = importer;
        }

        public bool CanImport(Code opcode)
        {
            return opcode == Code.Add ||
                   opcode == Code.Sub ||
                   opcode == Code.Mul ||
                   opcode == Code.Div ||
                   opcode == Code.Rem ||
                   opcode == Code.Div_Un ||
                   opcode == Code.Rem_Un;
        }

        public void Import(Instruction instruction, ImportContext context)
        {
            var op2 = _importer.PopExpression();
            var op1 = _importer.PopExpression();

            // StackValueKind is carefully ordered to make this work
            StackValueKind kind;
            if (op1.Kind > op2.Kind)
            {
                kind = op1.Kind;
            }
            else
            {
                kind = op2.Kind;
            }

            if (kind != StackValueKind.Int32)
            {
                throw new NotSupportedException("Binary operations on types other than int32 not supported yet");
            }

            var opcode = instruction.OpCode.Code;
            if (opcode < Code.Add || opcode > Code.Rem_Un)
            {
                throw new NotImplementedException();
            }
            Operation binaryOp = Operation.Add + (opcode - Code.Add);
            var binaryExpr = new BinaryOperator(binaryOp, op1, op2, kind);
            _importer.PushExpression(binaryExpr);
        }
    }
}
