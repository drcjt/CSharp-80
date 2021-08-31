using dnlib.DotNet.Emit;

namespace ILCompiler.Compiler.Importer
{
    public class NopImporter : IOpcodeImporter
    {
        public bool CanImport(Code opcode) => opcode == Code.Nop;

        public void Import(Instruction instruction, ImportContext context)
        {
            // Nothing to do
        }
    }
}
