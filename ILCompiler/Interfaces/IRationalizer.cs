using ILCompiler.Compiler;

namespace ILCompiler.Interfaces
{
    internal interface IRationalizer : IPhase
    {
        void Rationalize(MethodCompiler compiler);
    }
}
