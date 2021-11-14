using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System;

namespace ILCompiler.Compiler.Importer
{
    public class BinaryOperationImporter : IOpcodeImporter
    {
        public bool CanImport(Code code)
        {
            return code == Code.Add ||
                   code == Code.Sub ||
                   code == Code.Mul ||
                   code == Code.Div ||
                   code == Code.Rem ||
                   code == Code.Div_Un ||
                   code == Code.Rem_Un;
        }

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var op2 = importer.PopExpression();
            var op1 = importer.PopExpression();

            // StackValueKind is carefully ordered to make this work
            StackValueKind kind;
            kind = op1.Kind > op2.Kind ? op1.Kind : op2.Kind;

            if (kind != StackValueKind.Int32 && kind != StackValueKind.NativeInt)
            {
                throw new NotSupportedException($"Binary operation on type {kind} not supported");
            }

            Operation binaryOp = Operation.Add + (instruction.OpCode.Code - Code.Add);
            var binaryExpr = new BinaryOperator(binaryOp, isComparison: false, op1, op2, kind);
            importer.PushExpression(binaryExpr);
        }
    }
}
