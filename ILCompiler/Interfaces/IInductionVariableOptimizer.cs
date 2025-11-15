using ILCompiler.Compiler;

namespace ILCompiler.Interfaces
{
    public interface IInductionVariableOptimizer : IPhase
    {
        void Run(MethodCompiler compiler);
    }
}
