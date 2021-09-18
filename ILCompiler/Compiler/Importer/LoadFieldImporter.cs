using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System;

namespace ILCompiler.Compiler.Importer
{
    public class LoadFieldImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Ldfld;

        public void Import(Instruction instruction, ImportContext context, IILImporter importer)
        {
            var fieldDef = instruction.Operand as FieldDef;

            var obj = importer.PopExpression();

            if (obj.Kind == StackValueKind.ValueType)
            {
                obj = new LocalVariableAddressEntry((obj as LocalVariableEntry).LocalNumber);
            }

            if (obj.Kind != StackValueKind.ObjRef && obj.Kind != StackValueKind.ByRef)
            {
                throw new NotImplementedException($"LoadFieldImporter does not support {obj.Kind}");
            }

            var fieldSize = fieldDef.FieldType.GetExactSize(false);
            var kind = fieldDef.FieldType.GetStackValueKind();
            var node = new FieldEntry(obj, fieldDef.FieldOffset, fieldSize, kind);
            importer.PushExpression(node);
        }
    }
}
