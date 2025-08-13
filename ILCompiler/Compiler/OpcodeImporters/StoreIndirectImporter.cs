using ILCompiler.TypeSystem.Common;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class StoreIndirectImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
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
                    var typeDesc = (TypeDesc)instruction.Operand;
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

            var value = importer.Pop();
            return ImportStoreIndValue(importer, type, exactSize, value);
        }

        public static bool ImportStoreIndValue(IImporter importer, VarType type, int exactSize, StackEntry value)
        {
            var addr = importer.Pop();

            if (addr.Type == VarType.Int)
            {
                var cast = importer.CodeFolder.FoldExpression(new CastEntry(addr, VarType.Ptr));
                addr = cast;
            }

            if (type.IsSmall() && !value.Type.IsSmall())
            {
                value = importer.CodeFolder.FoldExpression(new CastEntry(value, type));
            }

            StackEntry node = new StoreIndEntry(addr, value, value.Type, fieldOffset: 0, exactSize);

            if (value.Type == VarType.Struct)
            {
                node = importer.StoreStruct(node);
            }

            importer.ImportAppendTree(node);

            return true;
        }
    }
}
