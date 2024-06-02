using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.Common;
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

            var field = context.Module.Create((IField)instruction.Operand);

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
