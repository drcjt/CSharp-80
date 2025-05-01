using ILCompiler.Compiler;
using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Interfaces
{
    public interface IFlowgraph : IPhase
    {
        void SetBlockOrder(IList<BasicBlock> blocks);
        void SetStatementSequence(Statement statement);
    }
}
