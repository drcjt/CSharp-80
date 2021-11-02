using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    public class SwitchCodeGenerator
    {
        public static void GenerateCode(SwitchEntry entry, Assembler assembler)
        {
            assembler.Pop(R16.HL);
            assembler.Pop(R16.HL);

            assembler.Ld(R8.A, R8.L);

            for (int targetIndex = 0; targetIndex < entry.JumpTable.Count; targetIndex++)
            {
                assembler.Or(R8.A);
                assembler.Jp(Condition.Zero, entry.JumpTable[targetIndex]);

                if (targetIndex < entry.JumpTable.Count - 1)
                {
                    assembler.Dec(R8.A);
                }
            }
        }
    }
}
