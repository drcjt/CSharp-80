using dnlib.DotNet.Emit;
using ILCompiler.IL.Stubs;
using ILCompiler.TypeSystem.Common;

namespace ILCompiler.IL
{
    public class RTILProvider : ILProvider
    {
        public override CilBody? GetMethodIL(MethodDesc method)
        {
            if (method.IsIntrinsic)
            {
                return TryGetIntrinsicMethodIL(method);
            }

            return null;
        }

        private static CilBody? TryGetIntrinsicMethodIL(MethodDesc method)
        {
            var declaringType = method.OwningType;
            switch (declaringType.Name)
            {
                case "Unsafe":
                    {
                        if (declaringType.Namespace == "Internal.Runtime.CompilerServices")
                        {
                            return UnsafeIntrinsics.EmitIL(method);
                        }
                    }
                    break;
            }

            return null;
        }
    }
}
