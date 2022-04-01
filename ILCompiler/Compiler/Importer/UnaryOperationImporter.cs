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
            importer.PushExpression(new UnaryOperator(unaryOp, importer.PopExpression()));
        }
    }
}
