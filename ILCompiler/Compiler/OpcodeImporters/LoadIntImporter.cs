using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class LoadIntImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IImporter importer)
        {
            long value;
            switch (instruction.Opcode)
            {
                case ILOpcode.ldc_i4_m1:
                    value = -1;
                    break;
                case ILOpcode.ldc_i4:
                    value = (int)instruction.Operand;
                    break;
                case ILOpcode.ldc_i4_s:
                    value = (sbyte)instruction.Operand;
                    break;

                case ILOpcode.ldc_i4_0:
                case ILOpcode.ldc_i4_1:
                case ILOpcode.ldc_i4_2:
                case ILOpcode.ldc_i4_3:
                case ILOpcode.ldc_i4_4:
                case ILOpcode.ldc_i4_5:
                case ILOpcode.ldc_i4_6:
                case ILOpcode.ldc_i4_7:
                case ILOpcode.ldc_i4_8:
                    value = instruction.Opcode - ILOpcode.ldc_i4_0;
                    break;

                default:
                    return false;
            }
            importer.Push(new Int32ConstantEntry(checked((int)value)));

            return true;
        }
    }
}
