using ILCompiler.Compiler;
using ILCompiler.Compiler.Dominators;

namespace ILCompiler.Interfaces
{
    internal interface IComputeDominators : IPhase
    {
        FlowgraphDominatorTree Build(MethodCompiler compiler);
    }
}
