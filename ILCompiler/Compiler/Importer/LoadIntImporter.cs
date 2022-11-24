using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class LoadIntImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            long value;
            switch (instruction.OpCode.Code)
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

                case Code.Ldc_I4_0:
                case Code.Ldc_I4_1:
                case Code.Ldc_I4_2:
                case Code.Ldc_I4_3:
                case Code.Ldc_I4_4:
                case Code.Ldc_I4_5:
                case Code.Ldc_I4_6:
                case Code.Ldc_I4_7:
                case Code.Ldc_I4_8:
                    value = instruction.OpCode.Code - Code.Ldc_I4_0;
                    break;

                default:
                    return false;
            }
            importer.PushExpression(new Int32ConstantEntry(checked((int)value)));

            return true;
        }
    }
}
