using ILCompiler.Compiler;
using ILCompiler.Compiler.Dominators;
using ILCompiler.Compiler.FlowgraphHelpers;

namespace ILCompiler.Interfaces
{
    internal interface IComputeDominators : IPhase
    {
        FlowgraphDominatorTree Build(FlowgraphDfsTree dfs, IList<BasicBlock> blocks);
    }
}
