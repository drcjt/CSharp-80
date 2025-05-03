using ILCompiler.Compiler;

namespace ILCompiler.Interfaces
{
    internal interface IEarlyValuePropagation : IPhase
    {
        void Run(IList<BasicBlock> blocks, LocalVariableTable locals);
    }
}