using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class UnaryOperationImporter : IOpcodeImporter
    {
        public bool CanImport(Code opcode) => opcode == Code.Neg || opcode == Code.Not;

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var unaryOp = Operation.Neg + (instruction.OpCode.Code - Code.Neg);
            var op1 = importer.PopExpression();
            var node = new UnaryOperator(unaryOp, op1);

            importer.PushExpression(node);
        }
    }
}
