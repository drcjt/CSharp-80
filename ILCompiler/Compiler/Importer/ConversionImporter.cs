using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System;

namespace ILCompiler.Compiler.Importer
{
    public class ConversionImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Conv_I2 || code == Code.Conv_U2 || code == Code.Conv_U1;

        public void Import(Instruction instruction, ImportContext context, IILImporter importer)
        {
            var unsigned = instruction.OpCode.Code == Code.Conv_U2 || instruction.OpCode.Code == Code.Conv_U1;

            WellKnownType wellKnownType;
            switch (instruction.OpCode.Code)
            {
                case Code.Conv_I2: wellKnownType = WellKnownType.Int16; break;
                case Code.Conv_U2: wellKnownType = WellKnownType.UInt16; break;
                case Code.Conv_U1: wellKnownType = WellKnownType.Byte; break;
                default: throw new NotImplementedException($"Conversion type not supported for opcode {instruction.OpCode.Code}");
            }
            
            var op1 = importer.PopExpression();
            op1 = new CastEntry(wellKnownType, unsigned, op1);
            importer.PushExpression(op1);
        }
    }
}
