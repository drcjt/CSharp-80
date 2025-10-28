using ILCompiler.Compiler;
using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Interfaces
{
    public interface IFlowgraph : IPhase
    {
        void SetBlockOrder(IList<BasicBlock> blocks);
        void SetStatementSequence(Statement statement);

        void InsertStatementAfter(BasicBlock block, Statement insertionPoint, Statement statement);
        void InsertStatementNearEnd(BasicBlock block, Statement statement);
        void InsertStatementAtEnd(BasicBlock block, Statement statement);
        void RemoveStatement(BasicBlock block, Statement statement);
    }
}
