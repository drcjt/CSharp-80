using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class ConversionImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) =>
            code == Code.Conv_U4 ||
            code == Code.Conv_I4 ||
            code == Code.Conv_U2 ||
            code == Code.Conv_I2 ||
            code == Code.Conv_U1 ||
            code == Code.Conv_I1 ||
            code == Code.Conv_U ||
            code == Code.Conv_I;

        private static VarType GetType(Code code)
        {
            return code switch
            {
                Code.Conv_U4 => VarType.UInt,
                Code.Conv_I4 => VarType.Int,
                Code.Conv_U2 => VarType.UShort,
                Code.Conv_I2 => VarType.Short,
                Code.Conv_U1 => VarType.Byte,
                Code.Conv_I1 => VarType.SByte,
                Code.Conv_U => VarType.Ptr,
                Code.Conv_I => VarType.Ptr,
                _ => throw new NotImplementedException(),
            };
        }

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            var desiredType = GetType(instruction.OpCode.Code);
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
            else if (op1.Type == VarType.Ptr && (desiredType == VarType.UInt || desiredType == VarType.Int))
            {
                var cast = new CastEntry(op1, desiredType);
                op1 = cast;
            }

            // TODO: Work out why this was added
            //op1.Type = GetType(instruction.OpCode.Code);

            importer.PushExpression(op1);
        }
    }
}