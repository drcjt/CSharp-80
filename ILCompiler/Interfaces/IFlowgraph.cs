using ILCompiler.Compiler;

namespace ILCompiler.Interfaces
{
    public interface IFlowgraph : IPhase
    {
        void SetBlockOrder(IList<BasicBlock> blocks);
    }
}
