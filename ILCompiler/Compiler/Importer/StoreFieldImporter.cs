using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem;
using ILCompiler.Common.TypeSystem.IL;
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

            var kind = fieldDef.FieldType.GetStackValueKind();

            if (value.Kind != StackValueKind.Int32 && value.Kind != StackValueKind.ValueType && value.Kind != StackValueKind.NativeInt)
            {
                throw new NotSupportedException($"Storing to field of type {value.Kind} not supported");
            }

            // Ensure fields have all offsets calculated
            if (fieldDef.FieldOffset == null)
            {
                fieldDef.DeclaringType.ToTypeSig().GetExactSize();
            }

            // TODO: Can this be removed
            var fieldSize = fieldDef.FieldType.GetExactSize();
            var fieldOffset = fieldDef.FieldOffset ?? 0;

            var node = new StoreIndEntry(addr, value, WellKnownType.Int32, fieldOffset, fieldSize);
            var varType = fieldDef.FieldType.GetVarType();
            node.Type = varType;

            importer.ImportAppendTree(node);
        }
    }
}
