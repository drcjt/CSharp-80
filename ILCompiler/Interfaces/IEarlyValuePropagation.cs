using ILCompiler.Compiler;

namespace ILCompiler.Interfaces
{
    internal interface IEarlyValuePropagation : IPhase
    {
        void Run(MethodCompiler compiler);
    }
}
