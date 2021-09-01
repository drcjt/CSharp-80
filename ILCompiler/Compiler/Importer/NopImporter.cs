using dnlib.DotNet.Emit;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class NopImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Nop;

        public void Import(Instruction instruction, ImportContext context, IILImporter importer)
        {
            // Nothing to do
        }
    }
}
