using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class StoreFieldImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.OpCode.Code != Code.Stfld && instruction.OpCode.Code != Code.Stsfld) return false;

            var isStoreStatic = instruction.OpCode == OpCodes.Stsfld;

            var fieldDefOrRef = instruction.Operand as IField;
            var fieldDef = fieldDefOrRef.ResolveFieldDefThrow();

            var value = importer.PopExpression();

            StackEntry addr;
            if (isStoreStatic)
            {
                var mangledFieldName = context.NameMangler.GetMangledFieldName(fieldDef);
                addr = new StaticFieldEntry(mangledFieldName);

                addr = ImportInitClass(fieldDef, context, importer, addr);
            }
            else
            {
                addr = importer.PopExpression();
            }

            // Ensure fields have all offsets calculated
            if (fieldDef.FieldOffset == null)
            {
                fieldDef.DeclaringType.ToTypeSig().GetInstanceFieldSize();
            }

            // TODO: Can this be removed
            var fieldSize = fieldDef.FieldType.GetInstanceFieldSize();
            var fieldOffset = fieldDef.FieldOffset ?? 0;

            var node = new StoreIndEntry(addr, value, fieldDef.FieldType.GetVarType(), fieldOffset, fieldSize);

            importer.ImportAppendTree(node);

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
