using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.Importer
{
    public class LoadTokenImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.Opcode != ILOpcode.ldtoken) return false;

            var obj = instruction.Operand;

            if (obj is FieldDesc field)
            {
                var node = context.NodeFactory.FieldRvaDataNode(field);
                var token = new TokenEntry(field, node.Label);

                importer.PushExpression(token);
            }
            else
            {
                throw new NotImplementedException();
            }

            return true;
        }
    }
}
