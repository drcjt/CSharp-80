using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.IL.Stubs;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.Dnlib;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.IL
{
    public class RTILProvider : ILProvider
    {
        public override MethodIL? GetMethodIL(MethodDesc method, DnlibModule module)
        {
            if (method is DnlibMethod)
            {
                if (method.IsIntrinsic)
                {
                    var cilbody = TryGetIntrinsicMethodIL(method);
                    if (cilbody != null)
                    {
                        return new DnlibMethodIL(module, cilbody);
                    }
                }
            }
            else if (method is MethodForInstantiatedType || method is InstantiatedMethod)
            {
                if (method.IsIntrinsic)
                {
                    var methodIL = TryGetPerInstantiationIntrinsicMethodIL(method);
                    if (methodIL != null)
                        return methodIL;
                }
            }

            return null;
        }

        private static MethodIL? TryGetPerInstantiationIntrinsicMethodIL(MethodDesc method)
        {
            var declaringType = method.OwningType;
            switch (declaringType.Name)
            {
                case "EqualityComparerHelpers":
                    {
                        if (declaringType.Namespace == "System.Collections.Generic")
                        {
                            return EqualityComparerHelpersIntrinsics.EmitIL(method);
                        }
                    }
                    break;
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
