using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public interface IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer);
    }
}
