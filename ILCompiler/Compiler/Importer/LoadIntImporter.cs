using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class LoadIntImporter : IOpcodeImporter
    {
        private readonly IILImporter _importer;
        public LoadIntImporter(IILImporter importer)
        {
            _importer = importer;
        }

        public bool CanImport(Code opcode)
        {
            return opcode == Code.Ldc_I4_M1 ||
                   opcode == Code.Ldc_I4_0 ||
                   opcode == Code.Ldc_I4_1 ||
                   opcode == Code.Ldc_I4_2 ||
                   opcode == Code.Ldc_I4_3 ||
                   opcode == Code.Ldc_I4_4 ||
                   opcode == Code.Ldc_I4_5 ||
                   opcode == Code.Ldc_I4_6 ||
                   opcode == Code.Ldc_I4_7 ||
                   opcode == Code.Ldc_I4_8 ||
                   opcode == Code.Ldc_I4 ||
                   opcode == Code.Ldc_I4_S;
        }

        public void Import(Instruction instruction, ImportContext context)
        {
            var opcode = instruction.OpCode.Code;
            long value;
            switch (opcode)
            {
                case Code.Ldc_I4_M1:
                    value = -1;
                    break;

                case Code.Ldc_I4:
                    value = (int)instruction.Operand;
                    break;

                case Code.Ldc_I4_S:
                    value = (sbyte)instruction.Operand;
                    break;

                default:
                    value = opcode - Code.Ldc_I4_0;
                    break;
            }

            _importer.PushExpression(new Int32ConstantEntry(checked((int)value)));
        }
    }
}
