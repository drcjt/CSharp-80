using ILCompiler.Compiler;

namespace ILCompiler.Interfaces
{
    public interface ILoopFinder : IPhase
    {
        void FindLoops(IList<BasicBlock> blocks);
    }
}