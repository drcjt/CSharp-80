using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem;
using ILCompiler.Common.TypeSystem.IL;
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

        private VarType GetType(Code code)
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
            WellKnownType wellKnownType;
            switch (instruction.OpCode.Code)
            {
                case Code.Conv_I4: wellKnownType = WellKnownType.Int32; break;
                case Code.Conv_U4: wellKnownType = WellKnownType.UInt32; break;
                case Code.Conv_I2: wellKnownType = WellKnownType.Int16; break;
                case Code.Conv_U2: wellKnownType = WellKnownType.UInt16; break;
                case Code.Conv_I1: wellKnownType = WellKnownType.SByte; break;
                case Code.Conv_U1: wellKnownType = WellKnownType.Byte; break;
                case Code.Conv_U: wellKnownType = WellKnownType.UIntPtr; break;
                case Code.Conv_I: wellKnownType = WellKnownType.IntPtr; break;
                default: throw new NotImplementedException($"Conversion type not supported for opcode {instruction.OpCode.Code}");
            }

            var desiredType = GetType(instruction.OpCode.Code);
            var op1 = importer.PopExpression();

            // Work out if a cast is required
            if (op1.Type.IsInt() && desiredType.IsSmall())
            {
                var cast = new CastEntry(wellKnownType, op1, desiredType);
                cast.DesiredType2 = desiredType;
                op1 = cast;
            }
            else if (op1.Type.IsInt() && desiredType == VarType.Ptr)
            {
                var cast = new CastEntry(wellKnownType, op1, VarType.Ptr);
                cast.DesiredType2 = VarType.Ptr;
                op1 = cast;
            }

            op1.Type = GetType(instruction.OpCode.Code);

            importer.PushExpression(op1);
        }
    }
}