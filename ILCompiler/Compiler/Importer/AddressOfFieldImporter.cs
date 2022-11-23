﻿using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class AddressOfFieldImporter : SingleOpcodeImporter
    {
        protected override Code Code { get; } = Code.Ldflda;

        protected override void ImportOpcode(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var fieldDefOrRef = instruction.Operand as IField;
            var fieldDef = fieldDefOrRef.ResolveFieldDef();

            var obj = importer.PopExpression();

            if (obj.Type != VarType.Ref && obj.Type != VarType.ByRef)
            {
                throw new NotImplementedException($"LoadFieldImporter does not support {obj.Type}");
            }

            var node = new FieldAddressEntry(fieldDef.Name, obj, fieldDef?.FieldOffset ?? 0);

            importer.PushExpression(node);
        }
    }
}
