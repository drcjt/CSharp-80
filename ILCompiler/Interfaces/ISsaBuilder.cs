using ILCompiler.Compiler;
using System.Collections.Generic;

namespace ILCompiler.Interfaces
{
    public interface ISsaBuilder
    {
        public void Build(IList<BasicBlock> blocks);
    }
}