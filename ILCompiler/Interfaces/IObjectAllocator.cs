using ILCompiler.Compiler;

namespace ILCompiler.Interfaces
{
    internal interface IObjectAllocator : IPhase
    {
        public void DoPhase(IList<BasicBlock> blocks);
    }
}
