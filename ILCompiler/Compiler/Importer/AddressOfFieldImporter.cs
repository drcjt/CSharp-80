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
            if (instruction.OpCode.Code != Code.Ldflda) return false;

            var fieldDefOrRef = instruction.Operand as IField;
            var fieldDef = fieldDefOrRef.ResolveFieldDef();

            var obj = importer.PopExpression();

            if (obj.Type != VarType.Ref && obj.Type != VarType.ByRef)
            {
                throw new NotImplementedException($"LoadFieldImporter does not support {obj.Type}");
            }

            var node = new FieldAddressEntry(fieldDef.Name, obj, fieldDef?.FieldOffset ?? 0);

            importer.PushExpression(node);

            return true;
        }
    }
}
