using ILCompiler.Compiler;

namespace ILCompiler.Interfaces
{
    internal interface IMorpher : IPhase
    {
        void Morph(IList<BasicBlock> blocks);
    }
}
