using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class SwitchCodeGenerator : ICodeGenerator<SwitchEntry>
    {
        public void GenerateCode(SwitchEntry entry, CodeGeneratorContext context)
        {
            context.InstructionsBuilder.Pop(HL);      // LSW
            context.InstructionsBuilder.Pop(DE);      // Ignore MSW

            context.InstructionsBuilder.Ld(A, L);

            for (int targetIndex = 0; targetIndex < entry.JumpTable.Count; targetIndex++)
            {
                context.InstructionsBuilder.Or(A);
                context.InstructionsBuilder.Jp(Condition.Z, entry.JumpTable[targetIndex]);

                if (targetIndex < entry.JumpTable.Count - 1)
                {
                    context.InstructionsBuilder.Dec(A);
                }
            }
        }
    }
}
