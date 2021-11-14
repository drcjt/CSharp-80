using dnlib.DotNet.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class LoadIntImporter : IOpcodeImporter
    {
        public bool CanImport(Code code)
        {
            return code == Code.Ldc_I4_M1 ||
                   code == Code.Ldc_I4_0 ||
                   code == Code.Ldc_I4_1 ||
                   code == Code.Ldc_I4_2 ||
                   code == Code.Ldc_I4_3 ||
                   code == Code.Ldc_I4_4 ||
                   code == Code.Ldc_I4_5 ||
                   code == Code.Ldc_I4_6 ||
                   code == Code.Ldc_I4_7 ||
                   code == Code.Ldc_I4_8 ||
                   code == Code.Ldc_I4 ||
                   code == Code.Ldc_I4_S;
        }

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            importer.PushExpression(new Int32ConstantEntry(checked((int)GetValue(instruction))));
        }

        private static long GetValue(Instruction instruction)
        {
            var code = instruction.OpCode.Code;
            var value = code switch
            {
                Code.Ldc_I4_M1 => -1,
                Code.Ldc_I4 => (int)instruction.Operand,
                Code.Ldc_I4_S => (sbyte)instruction.Operand,
                _ => code - Code.Ldc_I4_0,
            };
            return value;
        }
    }
}
