using ILCompiler.Compiler;
using ILCompiler.TypeSystem.Common;

namespace ILCompiler.Interfaces
{
    internal interface IMorpher : IPhase
    {
        void Init(MethodDesc method, IList<BasicBlock> blocks);
        void Morph(IList<BasicBlock> blocks, LocalVariableTable locals);
    }
}
