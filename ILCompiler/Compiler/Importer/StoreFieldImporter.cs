using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class StoreFieldImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Stfld;

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
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
        }
    }
}
