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
            if (instruction.OpCode.Code != Code.Ldflda && instruction.OpCode.Code != Code.Ldsflda) return false;

            var isLoadStatic = instruction.OpCode == OpCodes.Ldsflda;

            var fieldDefOrRef = (IField)instruction.Operand;
            var fieldDesc = context.TypeSystemContext.Create((IField)instruction.Operand);

            if (isLoadStatic)
            {
                var mangledFieldName = context.NameMangler.GetMangledFieldName(fieldDesc);

                StackEntry obj = new SymbolConstantEntry(mangledFieldName);
                if (!context.PreinitializationManager.IsPreinitialized(fieldDesc.OwningType))
                {
                    obj = InitClassHelper.ImportInitClass(fieldDesc.OwningType, context, importer, obj);
                }
                importer.PushExpression(obj);
            }
            else
            {
                var obj = importer.PopExpression();

                if (obj.Type != VarType.Ref && obj.Type != VarType.ByRef && obj.Type != VarType.Ptr)
                {
                    throw new NotImplementedException($"LoadFieldImporter does not support {obj.Type}");
                }

                var node = new FieldAddressEntry(fieldDesc.Name, obj, (uint)fieldDesc.Offset.AsInt);
                importer.PushExpression(node);
            }

            return true;
        }
    }
}
