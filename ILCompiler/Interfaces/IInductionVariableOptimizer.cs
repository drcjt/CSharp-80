using ILCompiler.Compiler;

namespace ILCompiler.Interfaces
{
    public interface IInductionVariableOptimizer : IPhase
    {
        void Run(IList<BasicBlock> blocks, FlowGraphNaturalLoops loops, LocalVariableTable locals);
    }
}
