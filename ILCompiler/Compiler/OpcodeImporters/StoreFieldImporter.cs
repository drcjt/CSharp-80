using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class StoreFieldImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IImporter importer)
        {
            if (instruction.Opcode != ILOpcode.stfld && instruction.Opcode != ILOpcode.stsfld) return false;

            var isStoreStatic = instruction.Opcode == ILOpcode.stsfld;

            var field = (FieldDesc)instruction.Operand;

            var value = importer.Pop();

            StackEntry addr;
            if (isStoreStatic)
            {
                var staticsBase = context.NameMangler.GetMangledTypeName(field.OwningType) + "_statics";
                addr = new SymbolConstantEntry(staticsBase);

                if (!context.PreinitializationManager.IsPreinitialized(field.OwningType))
                {
                    addr = InitClassHelper.ImportInitClass(field.OwningType, context, importer, addr);
                }
            }
            else
            {
                addr = importer.Pop();
            }

            var fieldSize = field.FieldType.GetElementSize().AsInt;
            var fieldOffset = field.Offset.AsInt;

            var node = new StoreIndEntry(addr, value, field.FieldType.VarType, (uint)fieldOffset, fieldSize);

            importer.ImportAppendTree(node, true);

            return true;
        }
    }
}