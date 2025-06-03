using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class LoadStringImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IImporter importer)
        {
            if (instruction.Opcode != ILOpcode.ldstr) return false;

            var node = context.NodeFactory.SerializedStringObject((string)instruction.Operand);

            importer.Push(new SymbolConstantEntry(node.Label));

            return true;
        }
    }
}
