using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.Importer
{
    public class UnaryOperationImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            Operation unaryOp;
            switch (instruction.Opcode)
            {
                case ILOpcode.neg:
                case ILOpcode.not:
                    unaryOp = Operation.Neg + (instruction.Opcode - ILOpcode.neg);
                    break;
                default:
                    return false;
            }
            var op1 = importer.PopExpression();
            var node = new UnaryOperator(unaryOp, op1);

            importer.PushExpression(node);

            return true;
        }
    }
}