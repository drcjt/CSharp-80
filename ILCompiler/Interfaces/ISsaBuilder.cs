using ILCompiler.Compiler;
using System.Collections.Generic;

namespace ILCompiler.Interfaces
{
    public interface ISsaBuilder : IPhase
    {
        public void Build(IList<BasicBlock> blocks, IList<LocalVariableDescriptor> localVariableTable);
    }
}