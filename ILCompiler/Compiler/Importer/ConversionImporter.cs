using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class ConversionImporter : IOpcodeImporter
    {
        private readonly IILImporter _importer;

        public ConversionImporter(IILImporter importer)
        {
            _importer = importer;
        }

        public bool CanImport(Code opcode)
        {
            return opcode == Code.Conv_I2 ||
                   opcode == Code.Conv_U2;
        }

        public void Import(Instruction instruction, ImportContext context)
        {
            var unsigned = instruction.OpCode.Code == Code.Conv_U2;
            var wellKnownType = instruction.OpCode.Code == Code.Conv_I2 ? WellKnownType.Int16 : WellKnownType.UInt16;

            var op1 = _importer.PopExpression();
            op1 = new CastEntry(wellKnownType, unsigned, op1);
            _importer.PushExpression(op1);
        }
    }
}
