using dnlib.DotNet;
using ILCompiler.Compiler;

namespace ILCompiler.Interfaces
{
    public interface IILImporter : IPhase
    {
        public IList<BasicBlock> Import(int parameterCount, int? returnBufferArgIndex, MethodDef method, IList<LocalVariableDescriptor> localVariableTable);
    }
}
