using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class JumpTrueCodeGenerator : ICodeGenerator<JumpTrueEntry>
    {
        public void GenerateCode(JumpTrueEntry entry, CodeGeneratorContext context)
        {
            // Pop i4 from stack and jump if non zero
            context.Emitter.Pop(R16.HL);      // LSW
            context.Emitter.Ld(R8.A, 0);
            context.Emitter.Add(R8.A, R8.L);
            context.Emitter.Pop(R16.HL);      // MSW
            context.Emitter.Jp(Condition.NonZero, entry.TargetLabel);
        }
    }
}
