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
                addr = new SymbolConstantEntry(mangledFieldName);

                if (!context.PreinitializationManager.IsPreinitialized(fieldDef.DeclaringType))
                {
                    addr = InitClassHelper.ImportInitClass(fieldDef, context, importer, addr);
                }
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

            importer.ImportAppendTree(node, true);

            return true;
        }
    }
}
