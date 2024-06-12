using ILCompiler.TypeSystem.Common;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.Importer
{
    public class StoreIndirectImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            VarType type;
            int exactSize = 0;
            switch (instruction.Opcode)
            {
                case ILOpcode.stind_i1:
                    type = VarType.SByte;
                    break;

                case ILOpcode.stind_i2:
                    type = VarType.Short;
                    break;

                case ILOpcode.stind_i4:
                    type = VarType.Int;
                    break;

                case ILOpcode.stind_i:
                    type = VarType.Ptr;
                    break;

                case ILOpcode.stind_ref:
                    type = VarType.Ref;
                    break;

                case ILOpcode.stobj:
                    var typeDesc = (TypeDesc)instruction.GetOperand();
                    type = typeDesc.VarType;
                    exactSize = typeDesc.GetElementSize().AsInt;
                    break;

                default:
                    return false;
            }

            if (instruction.Opcode != ILOpcode.stobj)
            {
                exactSize = type.GetTypeSize();
            }

            var value = importer.PopExpression();
            return ImportStoreIndValue(importer, type, exactSize, value);
        }

        public static bool ImportStoreIndValue(IILImporterProxy importer, VarType type, int exactSize, StackEntry value)
        {
            var addr = importer.PopExpression();

            if (addr.Type == VarType.Int)
            {
                var cast = CodeFolder.FoldExpression(new CastEntry(addr, VarType.Ptr));
                addr = cast;
            }

            if (type.IsSmall() && !value.Type.IsSmall())
            {
                value = CodeFolder.FoldExpression(new CastEntry(value, type));
            }

            var node = new StoreIndEntry(addr, value, value.Type, fieldOffset: 0, exactSize);

            importer.ImportAppendTree(node);

            return true;
        }
    }
}