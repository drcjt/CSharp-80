using dnlib.DotNet;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    internal static class InitClassHelper
    {
        public static StackEntry ImportInitClass(FieldDef fieldDef, ImportContext context, IILImporterProxy importer, StackEntry obj)
        {
            // Get the static constructor if one exists
            var declaringType = fieldDef.DeclaringType;
            var staticConstructorMethod = declaringType.FindStaticConstructor();
            if (staticConstructorMethod == null)
            {
                return obj;
            }

            // Generate call to static constructor
            var targetMethod = context.NameMangler.GetMangledMethodName(staticConstructorMethod);
            var staticInitCall = new CallEntry(targetMethod, new List<StackEntry>(), VarType.Void, 0);
            return new CommaEntry(staticInitCall, obj);
        }
    }
}