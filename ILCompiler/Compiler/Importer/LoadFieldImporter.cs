﻿using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class LoadFieldImporter : SingleOpcodeImporter
    {
        protected override Code Code { get; } = Code.Ldfld;

        protected override void ImportOpcode(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var fieldDefOrRef = instruction.Operand as IField;
            var fieldDef = fieldDefOrRef.ResolveFieldDef();

            // Ensure fields have all offsets calculated
            if (fieldDef.FieldOffset == null)
            {
                fieldDef.DeclaringType.ToTypeSig().GetExactSize();
            }

            var fieldOffset = fieldDef.FieldOffset ?? 0;

            var obj = importer.PopExpression();

            if (obj.Type == VarType.Struct)
            {
                if (obj is LocalVariableEntry)
                {
                    obj = new LocalVariableAddressEntry((obj.As<LocalVariableEntry>()).LocalNumber);
                }
                else if (obj is IndirectEntry)
                {
                    // If the object is itself an IndirectEntry e.g. resulting from a Ldfld
                    // then we should merge the Ldfld's together

                    // e.g. Ldfld SimpleVector::N
                    //      Ldfld Nested::Length
                    // will get converted into a single IndirectEntry node with the field offset
                    // being the combination of the field offsets for N and Length

                    var previousIndirect = obj.As<IndirectEntry>();
                    fieldOffset = previousIndirect.Offset + fieldOffset;
                    obj = previousIndirect.Op1;
                }
            }

            if (obj.Type != VarType.Ref && obj.Type != VarType.ByRef && obj.Type != VarType.Ptr)
            {
                throw new NotImplementedException($"LoadFieldImporter does not support {obj.Type}");
            }

            var fieldSize = fieldDef.FieldType.GetExactSize();

            var node = new IndirectEntry(obj, fieldDef.FieldType.GetVarType(), fieldSize, fieldOffset);

            importer.PushExpression(node);
        }
    }
}
