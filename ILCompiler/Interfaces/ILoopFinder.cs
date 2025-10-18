using ILCompiler.Compiler;
using ILCompiler.Compiler.FlowgraphHelpers;

namespace ILCompiler.Interfaces
{
    public interface ILoopFinder : IPhase
    {
        FlowGraphNaturalLoops FindLoops(IList<BasicBlock> blocks, FlowgraphDfsTree dfs);
    }
}
