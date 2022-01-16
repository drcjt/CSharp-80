using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class JumpTrueCodeGenerator : ICodeGenerator<JumpTrueEntry>
    {
        public void GenerateCode(JumpTrueEntry entry, CodeGeneratorContext context)
        {
            // Pop i4 from stack and jump if non zero
            context.Assembler.Pop(R16.HL);      // LSW
            context.Assembler.Ld(R8.A, 0);
            context.Assembler.Add(R8.A, R8.L);
            context.Assembler.Pop(R16.HL);      // MSW
            context.Assembler.Jp(Condition.NonZero, entry.TargetLabel);
        }
    }
}
