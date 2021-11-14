using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System;

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
            
            var op1 = importer.PopExpression();

            // Work out if a cast is required
            if ((op1.Kind == StackValueKind.Int32 && wellKnownType == Common.TypeSystem.WellKnownType.UInt16) ||
                (op1.Kind == StackValueKind.Int32 && wellKnownType == Common.TypeSystem.WellKnownType.Int16) ||
                (op1.Kind == StackValueKind.Int32 && wellKnownType == WellKnownType.Byte) ||
                (op1.Kind == StackValueKind.Int32 && wellKnownType == WellKnownType.SByte))
            {
                op1 = new CastEntry(wellKnownType, op1);
            }

            importer.PushExpression(op1);
        }
    }
}
