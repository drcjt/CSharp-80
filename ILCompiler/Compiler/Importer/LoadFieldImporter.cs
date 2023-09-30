using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class LoadFieldImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.OpCode != OpCodes.Ldfld && instruction.OpCode != OpCodes.Ldsfld) return false;

            var isLoadStatic = instruction.OpCode == OpCodes.Ldsfld;

            var fieldDefOrRef = instruction.Operand as IField;
            var fieldDef = fieldDefOrRef.ResolveFieldDefThrow();

            // Ensure fields have all offsets calculated
            if (fieldDef.FieldOffset == null)
            {
                fieldDef.DeclaringType.ToTypeSig().GetInstanceFieldSize();
            }

            var fieldOffset = fieldDef.FieldOffset ?? 0;

            StackEntry obj;
            if (isLoadStatic)
            {
                var mangledFieldName = context.NameMangler.GetMangledFieldName(fieldDef);
                obj = new StaticFieldEntry(mangledFieldName);

                var declaringType = fieldDef.DeclaringType;

                if (context.PreinitializationManager.IsPreinitialized(declaringType))
                {
                    var info = context.PreinitializationManager.GetPreinitializationInfo(declaringType);

                    // TODO use the info somehow
                }
                else
                {
                    obj = InitClassHelper.ImportInitClass(fieldDef, context, importer, obj);
                }
            }
            else
            {
                obj = importer.PopExpression();

                if (obj.Type == VarType.Struct)
                {
                    obj = GetStructAddress(ref fieldOffset, obj, importer);
                }

                if (obj.Type != VarType.Ref && obj.Type != VarType.ByRef && obj.Type != VarType.Ptr)
                {
                    throw new NotImplementedException($"LoadFieldImporter does not support {obj.Type}");
                }
            }

            var fieldSize = fieldDef.FieldType.GetInstanceFieldSize();

            var node = new IndirectEntry(obj, fieldDef.FieldType.GetVarType(), fieldSize, fieldOffset);

            importer.PushExpression(node);

            return true;
        }

        private static StackEntry GetStructAddress(ref uint fieldOffset, StackEntry structVal, IILImporterProxy importer)
        {
            // If the object is a struct, what we really want is
            // for the field to operate on the address of the struct.

            // See Compiler::impGetStructAddr in importer.cpp in ryujit

            if (structVal is LocalVariableEntry)
            {
                return new LocalVariableAddressEntry((structVal.As<LocalVariableEntry>()).LocalNumber);
            }
            else if (structVal is IndirectEntry)
            {
                // If the object is itself an IndirectEntry e.g. resulting from a Ldfld
                // then we should merge the Ldfld's together

                // e.g. Ldfld SimpleVector::N
                //      Ldfld Nested::Length
                // will get converted into a single IndirectEntry node with the field offset
                // being the combination of the field offsets for N and Length

                var previousIndirect = structVal.As<IndirectEntry>();
                fieldOffset = previousIndirect.Offset + fieldOffset;
                return previousIndirect.Op1;
            }            

            // Copy the struct to a new temp local variable
            // and then return the address of the new temp local variable
            var lclNum = importer.GrabTemp(structVal.Type, structVal.ExactSize);
            var asg = new StoreLocalVariableEntry(lclNum, false, structVal);
            importer.ImportAppendTree(asg);

            return new LocalVariableAddressEntry(lclNum);
        }
    }
}
