using dnlib.DotNet;
using ILCompiler.Compiler;

namespace ILCompiler.Common.TypeSystem.IL
{
    internal static class HelperExtensions
    {
        private const string HelperTypesNamespace = "Internal.Runtime.CompilerHelpers";

        public static MethodDef GetHelperEntryPoint(this CorLibModuleProvider corLibModuleProvider, string typeName, string methodName)
        {
            var compilerHelpers = corLibModuleProvider.FindThrow($"{HelperTypesNamespace}.{typeName}");
            return compilerHelpers.FindMethod(methodName);
        }
    }
}
