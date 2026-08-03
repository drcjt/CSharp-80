using ILCompiler.Compiler;

namespace ILCompiler.Interfaces
{
    internal interface IRemoveUnreachableTryRegions : IPhase
    {
        void Run(MethodCompiler compiler);
    }
}
