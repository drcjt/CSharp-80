using ILCompiler.Compiler;

namespace ILCompiler.Interfaces
{
    internal interface IMorpher : IPhase
    {
        void Init(MethodCompiler compiler);
        void Morph();
    }
}
