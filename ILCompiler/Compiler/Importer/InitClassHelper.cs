using dnlib.DotNet;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    internal static class InitClassHelper
    {
        public static StackEntry ImportInitClass(IMemberDef memberDef, ImportContext context, IILImporterProxy importer, StackEntry obj)
        {
            // Get the static constructor if one exists
            var declaringType = memberDef.DeclaringType;
            var staticConstructorMethod = declaringType.FindStaticConstructor();
            if (staticConstructorMethod != null)
            {
                // Generate call to static constructor
                var targetMethod = context.NameMangler.GetMangledMethodName(staticConstructorMethod);
                var staticInitCall = new CallEntry(targetMethod, new List<StackEntry>(), VarType.Void, 0);
                obj = new CommaEntry(staticInitCall, obj);
            }

            return obj;
        }
    }
}