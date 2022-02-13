using ILCompiler.Compiler;
using System.Collections.Generic;

namespace ILCompiler.Interfaces
{
    internal interface IObjectAllocator : IPhase
    {
        public void DoPhase(IList<BasicBlock> blocks);
    }
}
