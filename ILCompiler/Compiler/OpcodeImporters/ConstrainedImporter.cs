using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class ConstrainedImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IImporter importer)
        {
            if (instruction.Opcode != ILOpcode.constrained) return false;

            context.Constrained = (TypeDesc)instruction.Operand;

            return true;
        }
    }
}