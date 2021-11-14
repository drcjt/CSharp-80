using dnlib.DotNet.Emit;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public interface IOpcodeImporter
    {
        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer);
        public bool CanImport(Code opcode);
    }
}
