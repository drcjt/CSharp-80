using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class StoreFieldImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
        {
            if (instruction.Opcode != ILOpcode.stfld && instruction.Opcode != ILOpcode.stsfld) return false;

            var isStoreStatic = instruction.Opcode == ILOpcode.stsfld;

            var field = (FieldDesc)instruction.Operand;

            var value = importer.Pop();

            var fieldSize = field.FieldType.GetElementSize().AsInt;
            var fieldOffset = field.Offset.AsInt;

            StackEntry fieldAddress;
            if (isStoreStatic)
            {
                if (field.HasRva)
                {
                    var fieldRvaDataNode = importer.NodeFactory.FieldRvaDataNode(field);
                    fieldAddress = new SymbolConstantEntry(fieldRvaDataNode.Label);
                    fieldOffset = 0;
                }
                else
                {
                    var staticsBase = importer.NameMangler.GetMangledTypeName(field.OwningType) + "_statics";
                    fieldAddress = new SymbolConstantEntry(staticsBase);
                }

                if (!importer.PreinitializationManager.IsPreinitialized(field.OwningType))
                {
                    fieldAddress = InitClassHelper.ImportInitClass(field.OwningType, importer, fieldAddress);
                }
            }
            else
            {
                fieldAddress = importer.Pop();
            }

            StackEntry node = new StoreIndEntry(fieldAddress, value, field.FieldType.VarType, (uint)fieldOffset, fieldSize);

            if (field.FieldType.VarType == VarType.Struct)
            {
                node = importer.StoreStruct(node);
            }

            importer.ImportAppendTree(node, true);

            return true;
        }
    }
}
