using dnlib.DotNet.Emit;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class NopImporter : SingleOpcodeImporter
    {
        protected override Code Code { get; } = Code.Nop;

        protected override void ImportOpcode(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            // Nothing to do
        }
    }
}
