using ILCompiler.Compiler;

namespace ILCompiler.Interfaces
{
    internal interface IInliner : IPhase
    {
        void Inline(IList<BasicBlock> blocks, LocalVariableTable locals, string inputFilePath);
    }
}
