using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class ConversionImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
        {
            VarType desiredType;
            switch (instruction.Opcode)
            {
                case ILOpcode.conv_i:
                case ILOpcode.conv_u:
                case ILOpcode.conv_ovf_i:   // TODO: deal with overflow
                case ILOpcode.conv_ovf_u:   // TODO: deal with overflow
                    desiredType = VarType.Ptr;
                    break;

                case ILOpcode.conv_u4:
                    desiredType = VarType.UInt;
                    break;

                case ILOpcode.conv_i4:
                    desiredType = VarType.Int;
                    break;

                case ILOpcode.conv_u2:
                    desiredType = VarType.UShort;
                    break;

                case ILOpcode.conv_i2:
                    desiredType = VarType.Short;
                    break;

                case ILOpcode.conv_u1:
                    desiredType = VarType.Byte;
                    break;

                case ILOpcode.conv_i1:
                    desiredType = VarType.SByte;
                    break;

                default:
                    return false;
            }

            var op1 = importer.Pop();

            // Work out if a cast is required
            if (op1.Type.IsInt() && desiredType.IsSmall())
            {
                var cast = new CastEntry(op1, desiredType);
                op1 = cast;
            }
            else if (op1.Type.IsInt() && desiredType == VarType.Ptr)
            {
                var cast = new CastEntry(op1, VarType.Ptr);
                op1 = cast;
            }
            else if (op1.Type == VarType.Ptr && (desiredType == VarType.UInt || desiredType == VarType.Int || desiredType.IsShort()))
            {
                var cast = new CastEntry(op1, desiredType);
                op1 = cast;
            }

            op1 = CodeFolder.FoldExpression(op1);

            // TODO: Work out why this was added
            //op1.Type = GetType(instruction.OpCode.Code);

            importer.Push(op1);

            return true;
        }
    }
}