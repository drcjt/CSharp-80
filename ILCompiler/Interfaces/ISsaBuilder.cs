using ILCompiler.Compiler;
using ILCompiler.Compiler.Dominators;
using ILCompiler.Compiler.FlowgraphHelpers;

namespace ILCompiler.Interfaces
{
    public interface ISsaBuilder : IPhase
    {
        public void Build(FlowgraphDominatorTree dominatorTree, IList<BasicBlock> blocks, LocalVariableTable locals, bool dumpSsa, FlowgraphDfsTree dfs);
    }
}
