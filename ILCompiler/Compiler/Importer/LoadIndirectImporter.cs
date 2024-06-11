using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.Importer
{
    public class LoadIndirectImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            VarType type;
            switch (instruction.Opcode)
            {
                case ILOpcode.ldind_i:
                    type = VarType.Ptr;
                    break;
                case ILOpcode.ldind_i1:
                    type = VarType.SByte;
                    break;
                case ILOpcode.ldind_u1:
                    type = VarType.Byte;
                    break;
                case ILOpcode.ldind_i2:
                    type = VarType.Short;
                    break;
                case ILOpcode.ldind_u2:
                    type = VarType.UShort;
                    break;
                case ILOpcode.ldind_u4:
                    type = VarType.UInt;
                    break;
                case ILOpcode.ldind_i4:
                    type = VarType.Int;
                    break;
                case ILOpcode.ldind_ref:
                    type = VarType.Ref;
                    break;
                case ILOpcode.ldobj:
                    var typeDesc = (TypeDesc)instruction.GetOperand();
                    type = typeDesc.VarType;
                    break;

                default:
                    return false;
            }
            var addr = importer.PopExpression();

            if (addr.Type == VarType.Int)
            {
                var cast = CodeFolder.FoldExpression(new CastEntry(addr, VarType.Ptr));
                addr = cast;
            }

            var exactSize = type.GetTypeSize();

            var node = new IndirectEntry(addr, type, exactSize);
            importer.PushExpression(node);

            return true;
        }
    }
}