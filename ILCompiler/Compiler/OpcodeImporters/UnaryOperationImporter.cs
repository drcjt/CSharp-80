using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class UnaryOperationImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
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
            var op1 = importer.Pop();
            var node = new UnaryOperator(unaryOp, op1);

            importer.Push(node);

            return true;
        }
    }
}