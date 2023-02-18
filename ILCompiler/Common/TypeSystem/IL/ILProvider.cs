using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.Common;

namespace ILCompiler.Common.TypeSystem.IL
{
    public abstract class ILProvider
    {
        public abstract CilBody? GetMethodIL(MethodDesc method);
    }
}
