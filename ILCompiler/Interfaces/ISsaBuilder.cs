using ILCompiler.Compiler;
using ILCompiler.Compiler.FlowgraphHelpers;

namespace ILCompiler.Interfaces
{
    public interface ISsaBuilder : IPhase
    {
        public void Build(IList<BasicBlock> blocks, LocalVariableTable locals, bool dumpSsa, FlowgraphDfsTree dfs);
    }
}
