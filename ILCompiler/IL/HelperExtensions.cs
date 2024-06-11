using ILCompiler.TypeSystem.Common;

namespace ILCompiler.IL
{
    internal static class HelperExtensions
    {
        private const string HelperTypesNamespace = "Internal.Runtime.CompilerHelpers";

        public static MethodDesc GetHelperEntryPoint(this TypeSystemContext context, string typeName, string methodName)
        {
            TypeDesc helperType = (TypeDesc)context.SystemModule!.GetType(HelperTypesNamespace, typeName);
            return helperType.GetMethods().First(x => x.Name == methodName);
        }

        public static MethodDesc GetHelperEntryPoint(this TypeSystemContext context, string typeNamespace, string typeName, string methodName)
        {
            TypeDesc helperType = (TypeDesc)context.SystemModule!.GetType(typeNamespace, typeName);
            return helperType.GetMethods().First(x => x.Name == methodName);
        }
    }
}