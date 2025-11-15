using ILCompiler.Compiler;

namespace ILCompiler.Interfaces
{
    internal interface IInliner : IPhase
    {
        void Inline(MethodCompiler compiler, string inputFilePath);
    }
}
