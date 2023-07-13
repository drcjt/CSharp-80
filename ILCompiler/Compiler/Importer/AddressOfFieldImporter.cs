using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class AddressOfFieldImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.OpCode.Code != Code.Ldflda && instruction.OpCode.Code != Code.Ldsflda) return false;

            var isLoadStatic = instruction.OpCode == OpCodes.Ldsflda;

            var fieldDefOrRef = instruction.Operand as IField;
            var fieldDef = fieldDefOrRef.ResolveFieldDefThrow();

            // Ensure fields have all offsets calculated
            if (fieldDef.FieldOffset == null)
            {
                fieldDef.DeclaringType.ToTypeSig().GetInstanceFieldSize();
            }

            if (isLoadStatic)
            {
                var mangledFieldName = context.NameMangler.GetMangledFieldName(fieldDef);
                var obj = ImportInitClass(fieldDef, context, importer, new StaticFieldEntry(mangledFieldName));
                importer.PushExpression(obj);
            }
            else
            {
                var obj = importer.PopExpression();

                if (obj.Type != VarType.Ref && obj.Type != VarType.ByRef)
                {
                    throw new NotImplementedException($"LoadFieldImporter does not support {obj.Type}");
                }

                var node = new FieldAddressEntry(fieldDef.Name, obj, fieldDef?.FieldOffset ?? 0);
                importer.PushExpression(node);
            }

            return true;
        }

        private static StackEntry ImportInitClass(FieldDef fieldDef, ImportContext context, IILImporterProxy importer, StackEntry obj)
        {
            // Get the static constructor if one exists
            var declaringType = fieldDef.DeclaringType;
            var staticConstructorMethod = declaringType.FindStaticConstructor();
            if (staticConstructorMethod == null)
            {
                return obj;
            }

            // Generate call to static constructor
            // TODO: NEED TO ENSURE THIS IS ONLY CALLED ONCE THOUGH.
            // idea is to modify code in static constructor so that at the end of the method it changes the initial code to a RET
            var targetMethod = context.NameMangler.GetMangledMethodName(staticConstructorMethod);
            var staticInitCall = new CallEntry(targetMethod, new List<StackEntry>(), VarType.Void, 0);
            return new CommaEntry(staticInitCall, obj, obj.Type);
        }
    }
}
