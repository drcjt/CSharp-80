using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class UnaryOperationImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            Operation unaryOp;
            switch (instruction.OpCode.Code)
            {
                case Code.Neg:
                case Code.Not:
                    unaryOp = Operation.Neg + (instruction.OpCode.Code - Code.Neg);
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
