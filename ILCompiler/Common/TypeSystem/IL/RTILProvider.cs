using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.Common;

namespace ILCompiler.Common.TypeSystem.IL
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
            var declaringType = method.DeclaringType;
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
