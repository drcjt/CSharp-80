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
        private readonly IILImporter _importer;
        public StoreFieldImporter(IILImporter importer)
        {
            _importer = importer;
        }

        public bool CanImport(Code opcode)
        {
            return opcode == Code.Stfld;
        }

        public void Import(Instruction instruction, ImportContext context)
        {
            var fieldDef = instruction.Operand as FieldDef;

            var value = _importer.PopExpression();
            var addr = _importer.PopExpression();

            var kind = fieldDef.FieldType.GetStackValueKind();

            if (value.Kind != StackValueKind.Int32 && value.Kind != StackValueKind.ValueType)
            {
                throw new NotSupportedException();
            }

            _importer.ImportAppendTree(new StoreIndEntry(addr, value, WellKnownType.Int32, fieldDef.FieldOffset));
        }
    }
}
