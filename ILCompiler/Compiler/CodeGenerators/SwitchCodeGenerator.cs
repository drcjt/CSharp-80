using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class SwitchCodeGenerator : ICodeGenerator<SwitchEntry>
    {
        public void GenerateCode(SwitchEntry entry, CodeGeneratorContext context)
        {
            context.Emitter.Pop(HL);      // LSW
            context.Emitter.Pop(DE);      // Ignore MSW

            context.Emitter.Ld(A, L);

            for (int targetIndex = 0; targetIndex < entry.JumpTable.Count; targetIndex++)
            {
                context.Emitter.Or(A);
                context.Emitter.Jp(Condition.Z, entry.JumpTable[targetIndex]);

                if (targetIndex < entry.JumpTable.Count - 1)
                {
                    context.Emitter.Dec(A);
                }
            }
        }
    }
}
