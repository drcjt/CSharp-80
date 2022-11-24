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
            if (instruction.OpCode.Code != Code.Stfld) return false;

            var fieldDef = instruction.OperandAs<FieldDef>();

            var value = importer.PopExpression();
            var addr = importer.PopExpression();

            // Ensure fields have all offsets calculated
            if (fieldDef.FieldOffset == null)
            {
                fieldDef.DeclaringType.ToTypeSig().GetExactSize();
            }

            // TODO: Can this be removed
            var fieldSize = fieldDef.FieldType.GetExactSize();
            var fieldOffset = fieldDef.FieldOffset ?? 0;

            var node = new StoreIndEntry(addr, value, fieldOffset, fieldSize);

            importer.ImportAppendTree(node);

            return true;
        }
    }
}
