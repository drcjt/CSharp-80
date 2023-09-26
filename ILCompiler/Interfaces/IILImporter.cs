using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Compiler;

namespace ILCompiler.Interfaces
{
    public interface IILImporter : IPhase
    {
        public IList<BasicBlock> Import(int parameterCount, int? returnBufferArgIndex, MethodDesc method, LocalVariableTable locals);
    }
}
