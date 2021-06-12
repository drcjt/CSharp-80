using ILCompiler.Compiler;

namespace ILCompiler.Interfaces
{
    public interface IILImporter
    {
        public INameMangler NameMangler { get; }
        public void AddToPendingBasicBlocks(BasicBlock block);
    }
}
