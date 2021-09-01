using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class ConversionImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Conv_I2 || code == Code.Conv_U2;

        public void Import(Instruction instruction, ImportContext context, IILImporter importer)
        {
            var unsigned = instruction.OpCode.Code == Code.Conv_U2;
            var wellKnownType = instruction.OpCode.Code == Code.Conv_I2 ? WellKnownType.Int16 : WellKnownType.UInt16;

            var op1 = importer.PopExpression();
            op1 = new CastEntry(wellKnownType, unsigned, op1);
            importer.PushExpression(op1);
        }
    }
}
