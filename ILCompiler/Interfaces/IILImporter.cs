using ILCompiler.Compiler;

namespace ILCompiler.Interfaces
{
    public interface IILImporter
    {
        public void AddToPendingBasicBlocks(BasicBlock block);
    }
}
