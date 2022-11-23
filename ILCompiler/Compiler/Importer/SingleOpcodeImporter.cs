using dnlib.DotNet.Emit;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public abstract class SingleOpcodeImporter : IOpcodeImporter
    {
        protected abstract Code Code { get; }

        protected abstract void ImportOpcode(Instruction instruction, ImportContext context, IILImporterProxy importer);

        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.OpCode.Code == Code)
            {
                ImportOpcode(instruction, context, importer);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
