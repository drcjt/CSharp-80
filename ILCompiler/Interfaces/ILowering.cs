using ILCompiler.Compiler;
using System.Collections.Generic;

namespace ILCompiler.Interfaces
{
    public interface ILowering : IPhase
    {
        public void Run(IList<BasicBlock> blocks);
    }
}
