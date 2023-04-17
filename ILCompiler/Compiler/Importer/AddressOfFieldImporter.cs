using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class AddressOfFieldImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.OpCode.Code != Code.Ldflda && instruction.OpCode.Code != Code.Ldsflda) return false;

            var isLoadStatic = instruction.OpCode == OpCodes.Ldsflda;

            var fieldDefOrRef = instruction.Operand as IField;
            var fieldDef = fieldDefOrRef.ResolveFieldDefThrow();

            // Ensure fields have all offsets calculated
            if (fieldDef.FieldOffset == null)
            {
                fieldDef.DeclaringType.ToTypeSig().GetInstanceFieldSize();
            }

            if (isLoadStatic)
            {
                var mangledFieldName = context.NameMangler.GetMangledFieldName(fieldDef);
                importer.PushExpression(new StaticFieldEntry(mangledFieldName));
            }
            else
            {
                var obj = importer.PopExpression();

                if (obj.Type != VarType.Ref && obj.Type != VarType.ByRef)
                {
                    throw new NotImplementedException($"LoadFieldImporter does not support {obj.Type}");
                }

                var node = new FieldAddressEntry(fieldDef.Name, obj, fieldDef?.FieldOffset ?? 0);
                importer.PushExpression(node);
            }

            return true;
        }
    }
}
