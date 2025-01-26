using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.Importer
{
    public class LoadStringImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.Opcode != ILOpcode.ldstr) return false;

            var node = context.NodeFactory.SerializedStringObject((string)instruction.Operand);

            importer.PushExpression(new SymbolConstantEntry(node.Label));

            return true;
        }
    }
}
