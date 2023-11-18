using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class ConversionImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            VarType desiredType;
            switch (instruction.OpCode.Code)
            {
                case Code.Conv_I:
                case Code.Conv_U:
                    desiredType = VarType.Ptr;
                    break;

                case Code.Conv_U4:
                    desiredType = VarType.UInt;
                    break;

                case Code.Conv_I4:
                    desiredType = VarType.Int;
                    break;

                case Code.Conv_U2:
                    desiredType = VarType.UShort;
                    break;

                case Code.Conv_I2:
                    desiredType = VarType.Short;
                    break;

                case Code.Conv_U1:
                    desiredType = VarType.Byte;
                    break;

                case Code.Conv_I1:
                    desiredType = VarType.SByte;
                    break;

                default:
                    return false;
            }

            var op1 = importer.PopExpression();

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

            importer.PushExpression(op1);

            return true;
        }
    }
}