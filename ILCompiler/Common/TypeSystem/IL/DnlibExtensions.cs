using dnlib.DotNet;

namespace ILCompiler.Common.TypeSystem.IL
{
    public static class DnlibExtensions
    {
        private const string CompilerIntrinsicAttribute = "System.Runtime.CompilerServices.IntrinsicAttribute";

        public static bool IsIntrinsic(this MethodDef method)
        {
            return method.HasCustomAttributes && method.CustomAttributes.IsDefined(CompilerIntrinsicAttribute);
        }
    }
}
