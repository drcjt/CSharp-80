using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System;

namespace ILCompiler.Compiler.Importer
{
    public class StoreFieldImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Stfld;

        public void Import(Instruction instruction, ImportContext context, IILImporter importer)
        {
            var fieldDef = instruction.Operand as FieldDef;

            var value = importer.PopExpression();
            var addr = importer.PopExpression();

            var kind = fieldDef.FieldType.GetStackValueKind();

            if (value.Kind != StackValueKind.Int32 && value.Kind != StackValueKind.ValueType)
            {
                throw new NotSupportedException();
            }
            // Ensure fields have all offsets calculated
            if (!fieldDef.FieldOffset.HasValue)
            {
                fieldDef.DeclaringType.ToTypeSig().GetExactSize();
            }

            var fieldSize = fieldDef.FieldType.GetExactSize();

            importer.ImportAppendTree(new StoreIndEntry(addr, value, WellKnownType.Int32, fieldDef.FieldOffset, fieldDef.FieldType.GetExactSize()));
        }
    }
}
