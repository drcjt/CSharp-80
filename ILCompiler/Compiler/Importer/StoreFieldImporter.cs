using ILCompiler.TypeSystem.Common;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.Importer
{
    public class StoreFieldImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.Opcode != ILOpcode.stfld && instruction.Opcode != ILOpcode.stsfld) return false;

            var isStoreStatic = instruction.Opcode == ILOpcode.stsfld;

            var field = (FieldDesc)instruction.Operand;

            var value = importer.PopExpression();

            StackEntry addr;
            if (isStoreStatic)
            {
                var mangledFieldName = context.NameMangler.GetMangledFieldName(field);
                addr = new SymbolConstantEntry(mangledFieldName);

                if (!context.PreinitializationManager.IsPreinitialized(field.OwningType))
                {
                    addr = InitClassHelper.ImportInitClass(field.OwningType, context, importer, addr);
                }
            }
            else
            {
                addr = importer.PopExpression();
            }

            var fieldSize = field.FieldType.GetElementSize().AsInt;
            var fieldOffset = isStoreStatic ? 0 : field.Offset.AsInt;

            var node = new StoreIndEntry(addr, value, field.FieldType.VarType, (uint)fieldOffset, fieldSize);

            importer.ImportAppendTree(node, true);

            return true;
        }
    }
}