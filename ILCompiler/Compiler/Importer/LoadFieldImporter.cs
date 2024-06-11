using ILCompiler.TypeSystem.Common;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.Importer
{
    public class LoadFieldImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.Opcode != ILOpcode.ldfld && instruction.Opcode != ILOpcode.ldsfld) return false;

            var isLoadStatic = instruction.Opcode == ILOpcode.ldsfld;

            var fieldDesc = (FieldDesc)instruction.GetOperand();

            uint fieldOffset = (uint)fieldDesc.Offset.AsInt;

            StackEntry obj;
            if (isLoadStatic)
            {
                var mangledFieldName = context.NameMangler.GetMangledFieldName(fieldDesc);
                obj = new SymbolConstantEntry(mangledFieldName);

                if (!context.PreinitializationManager.IsPreinitialized(fieldDesc.OwningType))
                {
                    obj = InitClassHelper.ImportInitClass(fieldDesc.OwningType, context, importer, obj);
                }
                fieldOffset = 0;
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

            var fieldSize = fieldDesc.FieldType.GetElementSize().AsInt;

            var node = new IndirectEntry(obj, fieldDesc.FieldType.VarType, fieldSize, fieldOffset);

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