using dnlib.DotNet.Emit;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.TypeSystem.Dnlib;

namespace ILCompiler.IL
{
    public class RTILProvider : ILProvider
    {
        public override MethodIL? GetMethodIL(MethodDesc method, DnlibModule module)
        {
            if (method.IsIntrinsic)
            {
                var cilbody = TryGetIntrinsicMethodIL(method);
                if (cilbody != null)
                {
                    return new DnlibMethodIL(module, cilbody);
                }
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
