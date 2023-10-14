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
            context.InstructionsBuilder.Pop(HL);      // LSW
            context.InstructionsBuilder.Ld(A, 0);
            context.InstructionsBuilder.Add(A, L);
            context.InstructionsBuilder.Pop(HL);      // MSW
            context.InstructionsBuilder.Jp(Condition.NZ, entry.TargetLabel);
        }
    }
}
