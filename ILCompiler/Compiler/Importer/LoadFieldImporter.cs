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
        private readonly IILImporter _importer;
        public LoadFieldImporter(IILImporter importer)
        {
            _importer = importer;
        }

        public bool CanImport(Code opcode)
        {
            return opcode == Code.Ldfld;
        }

        public void Import(Instruction instruction, ImportContext context)
        {
            var fieldDef = instruction.Operand as FieldDef;

            var obj = _importer.PopExpression();

            if (obj.Kind == StackValueKind.ValueType)
            {
                var localNode = obj as LocalVariableEntry;
                obj = new LocalVariableAddressEntry(localNode.LocalNumber);
            }

            if (obj.Kind != StackValueKind.ObjRef)
            {
                throw new NotImplementedException();
            }

            var fieldSize = fieldDef.FieldType.GetExactSize(false);
            var kind = fieldDef.FieldType.GetStackValueKind();
            var node = new FieldEntry(obj, fieldDef.FieldOffset, fieldSize, kind);
            _importer.PushExpression(node);
        }
    }
}
