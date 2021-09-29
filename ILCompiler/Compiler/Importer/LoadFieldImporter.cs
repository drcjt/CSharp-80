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
                if (obj is LocalVariableEntry)
                {
                    obj = new LocalVariableAddressEntry((obj as LocalVariableEntry).LocalNumber);
                }
                else if (obj is FieldEntry)
                {
                    // TODO: Need to work out what to do here.
                    // The field is effectively on top of the stack so need the address of this field
                    // Think the right answer is to merge the FieldEntry nodes together
                    throw new NotImplementedException("Nested Field Loads not yet implemented");
                }
            }

            if (obj.Kind != StackValueKind.ObjRef && obj.Kind != StackValueKind.ByRef)
            {
                throw new NotImplementedException($"LoadFieldImporter does not support {obj.Kind}");
            }

            var kind = fieldDef.FieldType.GetStackValueKind();
            var node = new FieldEntry(obj, fieldDef, kind);
            importer.PushExpression(node);
        }
    }
}
