using ILCompiler.Compiler;

namespace ILCompiler.Interfaces
{
    public interface ILoopFinder : IPhase
    {
        FlowGraphNaturalLoops FindLoops(MethodCompiler compiler);
    }
}
