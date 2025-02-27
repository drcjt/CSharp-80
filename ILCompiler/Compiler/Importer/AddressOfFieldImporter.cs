using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.Importer
{
    public class AddressOfFieldImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.Opcode != ILOpcode.ldflda && instruction.Opcode != ILOpcode.ldsflda) return false;

            var isLoadStatic = instruction.Opcode == ILOpcode.ldsflda;

            var fieldDesc = (FieldDesc)instruction.Operand;

            if (isLoadStatic)
            {
                var staticsBase = context.NameMangler.GetMangledTypeName(fieldDesc.OwningType) + "_statics";
                StackEntry obj = new SymbolConstantEntry(staticsBase, fieldDesc.Offset.AsInt);
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
