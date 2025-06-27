using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class LoadFieldImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
        {
            if (instruction.Opcode != ILOpcode.ldfld && instruction.Opcode != ILOpcode.ldsfld) return false;

            var isLoadStatic = instruction.Opcode == ILOpcode.ldsfld;

            var runtimeDeterminedType = (FieldDesc)instruction.Operand;

            uint fieldOffset = (uint)runtimeDeterminedType.Offset.AsInt;

            StackEntry obj;
            if (isLoadStatic)
            {
                var staticsBase = importer.NameMangler.GetMangledTypeName(runtimeDeterminedType.OwningType) + "_statics";
                obj = new SymbolConstantEntry(staticsBase);

                if (!importer.PreinitializationManager.IsPreinitialized(runtimeDeterminedType.OwningType))
                {
                    obj = InitClassHelper.ImportInitClass(runtimeDeterminedType.OwningType, importer, obj);
                }
            }
            else
            {
                obj = importer.Pop();

                if (obj.Type == VarType.Struct)
                {
                    obj = GetStructAddress(ref fieldOffset, obj, importer);
                }

                if (obj.Type != VarType.Ref && obj.Type != VarType.ByRef && obj.Type != VarType.Ptr)
                {
                    throw new NotImplementedException($"LoadFieldImporter does not support {obj.Type}");
                }
            }

            var fieldType = runtimeDeterminedType.FieldType.VarType;
            if (runtimeDeterminedType.OwningType.IsRuntimeDeterminedSubtype)
            {
                // TODO: will need to revist this to deal with runtime determined types properly
                // Use Ref for canonical types
                fieldType = VarType.Ref;
            }

            var fieldSize = runtimeDeterminedType.FieldType.GetElementSize().AsInt;

            var node = new IndirectEntry(obj, fieldType, fieldSize, fieldOffset);

            importer.Push(node);

            return true;
        }

        private static StackEntry GetStructAddress(ref uint fieldOffset, StackEntry structVal, IImporter importer)
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
            StackEntry asg = importer.NewTempStore(lclNum, structVal);

            importer.ImportAppendTree(asg);

            return new LocalVariableAddressEntry(lclNum);
        }
    }
}