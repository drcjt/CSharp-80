using System.Collections.Generic;

namespace ILCompiler.z80.Interfaces
{
    public interface IOptimizer
    {
        public void Optimize(IList<Instruction> instructions);
    }
}
