using ILCompiler.TypeSystem.Common;
using ILCompiler.Compiler;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Interfaces
{
    public interface IILImporter : IPhase
    {
        public IList<BasicBlock> Import(int parameterCount, int? returnBufferArgIndex, MethodDesc method, LocalVariableTable locals, IList<EHClause> ehClauses);
    }
}
