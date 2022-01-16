using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class SwitchCodeGenerator : ICodeGenerator<SwitchEntry>
    {
        public void GenerateCode(SwitchEntry entry, CodeGeneratorContext context)
        {
            context.Assembler.Pop(R16.HL);      // LSW
            context.Assembler.Pop(R16.DE);      // Ignore MSW

            context.Assembler.Ld(R8.A, R8.L);

            for (int targetIndex = 0; targetIndex < entry.JumpTable.Count; targetIndex++)
            {
                context.Assembler.Or(R8.A);
                context.Assembler.Jp(Condition.Zero, entry.JumpTable[targetIndex]);

                if (targetIndex < entry.JumpTable.Count - 1)
                {
                    context.Assembler.Dec(R8.A);
                }
            }
        }
    }
}
