using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class ShiftOperationImporter : IOpcodeImporter
    {
        public bool CanImport(Code opcode)
        {
            return opcode == Code.Shl || opcode == Code.Shr;
        }

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var op2 = importer.PopExpression();
            var op1 = importer.PopExpression(); // operand to be shifted

            var shiftOp = Operation.Lsh + (instruction.OpCode.Code - Code.Shl);
            var binaryExpr = new BinaryOperator(shiftOp, isComparison: false, op1, op2, VarType.Int);
            importer.PushExpression(binaryExpr);
        }
    }
}
