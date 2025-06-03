using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class AddressOfFieldImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IImporter importer)
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
                importer.Push(obj);
            }
            else
            {
                var obj = importer.Pop();

                if (obj.Type != VarType.Ref && obj.Type != VarType.ByRef && obj.Type != VarType.Ptr)
                {
                    throw new NotImplementedException($"LoadFieldImporter does not support {obj.Type}");
                }

                var node = new FieldAddressEntry(fieldDesc.Name, obj, (uint)fieldDesc.Offset.AsInt);
                importer.Push(node);
            }

            return true;
        }
    }
}
