using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class LoadTokenImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
        {
            if (instruction.Opcode != ILOpcode.ldtoken) return false;

            var obj = instruction.Operand;

            if (obj is FieldDesc field)
            {
                var node = importer.NodeFactory.FieldRvaDataNode(field);
                var token = new TokenEntry(field, node.Label);

                importer.Push(token);
            }
            else
            {
                throw new NotImplementedException();
            }

            return true;
        }
    }
}
