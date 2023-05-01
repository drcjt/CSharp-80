using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class JumpTrueCodeGenerator : ICodeGenerator<JumpTrueEntry>
    {
        public void GenerateCode(JumpTrueEntry entry, CodeGeneratorContext context)
        {
            // Pop i4 from stack and jump if non zero
            context.Emitter.Pop(HL);      // LSW
            context.Emitter.Ld(A, 0);
            context.Emitter.Add(A, L);
            context.Emitter.Pop(HL);      // MSW
            context.Emitter.Jp(Condition.NZ, entry.TargetLabel);
        }
    }
}
