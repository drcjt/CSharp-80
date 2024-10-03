using ILCompiler.Compiler;

namespace ILCompiler.Interfaces
{
    public interface ILowering : IPhase
    {
        public void Run(IList<BasicBlock> blocks, LocalVariableTable locals);
    }
}
