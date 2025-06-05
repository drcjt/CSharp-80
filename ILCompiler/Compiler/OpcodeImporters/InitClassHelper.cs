using ILCompiler.TypeSystem.Common;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.OpcodeImporters
{
    internal static class InitClassHelper
    {
        public static StackEntry ImportInitClass(TypeDesc type, IImporter importer, StackEntry obj)
        {
            // Get the static constructor if one exists
            var staticConstructorMethod = type.GetStaticConstructor();
            if (staticConstructorMethod != null && staticConstructorMethod.FullName != importer.Method.FullName)
            {
                // Generate call to static constructor
                var targetMethod = importer.NameMangler.GetMangledMethodName(staticConstructorMethod);
                var staticInitCall = new CallEntry(targetMethod, new List<StackEntry>(), VarType.Void, 0);
                obj = new CommaEntry(staticInitCall, obj);
            }

            return obj;
        }
    }
}