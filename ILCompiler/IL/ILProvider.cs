using dnlib.DotNet.Emit;
using ILCompiler.TypeSystem.Common;

namespace ILCompiler.IL
{
    public abstract class ILProvider
    {
        public abstract CilBody? GetMethodIL(MethodDesc method);
    }
}
