using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class SwitchCodeGenerator : ICodeGenerator<SwitchEntry>
    {
        public void GenerateCode(SwitchEntry entry, CodeGeneratorContext context)
        {
            context.Emitter.Pop(R16.HL);      // LSW
            context.Emitter.Pop(R16.DE);      // Ignore MSW

            context.Emitter.Ld(R8.A, R8.L);

            for (int targetIndex = 0; targetIndex < entry.JumpTable.Count; targetIndex++)
            {
                context.Emitter.Or(R8.A);
                context.Emitter.Jp(Condition.Zero, entry.JumpTable[targetIndex]);

                if (targetIndex < entry.JumpTable.Count - 1)
                {
                    context.Emitter.Dec(R8.A);
                }
            }
        }
    }
}
