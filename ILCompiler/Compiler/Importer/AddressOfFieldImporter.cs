using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System;

namespace ILCompiler.Compiler.Importer
{
    public class AddressOfFieldImporter : IOpcodeImporter
    {
        public bool CanImport(Code opcode)
        {
            return opcode == Code.Ldflda;
        }

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var fieldDefOrRef = instruction.Operand as IField;
            var fieldDef = fieldDefOrRef.ResolveFieldDef();

            var obj = importer.PopExpression();


            if (obj.Kind != StackValueKind.ObjRef && obj.Kind != StackValueKind.ByRef)
            {
                throw new NotImplementedException($"LoadFieldImporter does not support {obj.Kind}");
            }

            var node = new FieldAddressEntry(fieldDef.Name, obj, fieldDef?.FieldOffset ?? 0);

            importer.PushExpression(node);
        }
    }
}
