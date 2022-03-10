using ILCompiler.Compiler;

namespace ILCompiler.Interfaces
{
    public interface ISsaBuilder : IPhase
    {
        public void Build(IList<BasicBlock> blocks, IList<LocalVariableDescriptor> localVariableTable);
    }
}