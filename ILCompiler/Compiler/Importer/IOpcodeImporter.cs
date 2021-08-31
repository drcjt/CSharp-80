using dnlib.DotNet.Emit;

namespace ILCompiler.Compiler.Importer
{
    public interface IOpcodeImporter
    {
        public void Import(Instruction instruction, ImportContext context);
        public bool CanImport(Code opcode);
    }
}
