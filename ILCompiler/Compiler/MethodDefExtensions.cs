using dnlib.DotNet;

namespace ILCompiler.Compiler
{
    public static class MethodDefExtensions
    {
        private const string CompilerIntrinsicAttribute = "System.Runtime.CompilerServices.IntrinsicAttribute";

        public static bool IsIntrinsic(this MethodDef method)
        {
            return method.HasCustomAttributes && method.CustomAttributes.IsDefined(CompilerIntrinsicAttribute);
        }
    }
}
