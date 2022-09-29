using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class LoadFieldImporter : IOpcodeImporter
    {
        public bool CanImport(Code opcode) => opcode == Code.Ldfld;

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var fieldDefOrRef = instruction.Operand as IField;
            var fieldDef = fieldDefOrRef.ResolveFieldDef();

            // Ensure fields have all offsets calculated
            if (fieldDef.FieldOffset == null)
            {
                fieldDef.DeclaringType.ToTypeSig().GetExactSize();
            }

            var fieldOffset = fieldDef.FieldOffset ?? 0;

            var obj = importer.PopExpression();

            if (obj.Kind == StackValueKind.ValueType)
            {
                if (obj is LocalVariableEntry)
                {
                    obj = new LocalVariableAddressEntry((obj.As<LocalVariableEntry>()).LocalNumber);
                }
                else if (obj is IndirectEntry)
                {
                    // If the object is itself an IndirectEntry e.g. resulting from a Ldfld
                    // then we should merge the Ldfld's together

                    // e.g. Ldfld SimpleVector::N
                    //      Ldfld Nested::Length
                    // will get converted into a single IndirectEntry node with the field offset
                    // being the combination of the field offsets for N and Length

                    var previousIndirect = obj.As<IndirectEntry>();
                    fieldOffset = previousIndirect.Offset + fieldOffset;
                    obj = previousIndirect.Op1;
                }
            }

            if (obj.Kind != StackValueKind.ObjRef && obj.Kind != StackValueKind.ByRef && obj.Kind != StackValueKind.NativeInt)
            {
                throw new NotImplementedException($"LoadFieldImporter does not support {obj.Kind}");
            }

            var kind = fieldDef.FieldType.GetStackValueKind();
            var fieldSize = fieldDef.FieldType.GetExactSize();

            var node = new IndirectEntry(obj, kind, fieldSize, fieldSize, fieldOffset);

            var varType = fieldDef.FieldType.GetVarType();
            node.Type = varType;

            importer.PushExpression(node);
        }
    }
}
