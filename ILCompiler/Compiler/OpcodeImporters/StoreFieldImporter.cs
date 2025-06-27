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

            StackEntry addr;
            if (isStoreStatic)
            {
                var staticsBase = importer.NameMangler.GetMangledTypeName(field.OwningType) + "_statics";
                addr = new SymbolConstantEntry(staticsBase);

                if (!importer.PreinitializationManager.IsPreinitialized(field.OwningType))
                {
                    addr = InitClassHelper.ImportInitClass(field.OwningType, importer, addr);
                }
            }
            else
            {
                addr = importer.Pop();
            }

            var fieldSize = field.FieldType.GetElementSize().AsInt;
            var fieldOffset = field.Offset.AsInt;

            StackEntry node = new StoreIndEntry(addr, value, field.FieldType.VarType, (uint)fieldOffset, fieldSize);

            if (field.FieldType.VarType == VarType.Struct)
            {
                node = importer.StoreStruct(node);
            }

            importer.ImportAppendTree(node, true);

            return true;
        }
    }
}