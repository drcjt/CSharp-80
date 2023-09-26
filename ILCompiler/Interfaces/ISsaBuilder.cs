using ILCompiler.Compiler;

namespace ILCompiler.Interfaces
{
    public interface ISsaBuilder : IPhase
    {
        public void Build(IList<BasicBlock> blocks, LocalVariableTable locals, bool dumpSsa);
    }
}