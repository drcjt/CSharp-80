using ILCompiler.Compiler;
using System.Collections.Generic;

namespace ILCompiler.Interfaces
{
    public interface IFlowgraph : IPhase
    {
        void SetBlockOrder(IList<BasicBlock> blocks);
    }
}
