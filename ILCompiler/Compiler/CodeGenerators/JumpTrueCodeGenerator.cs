using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    public class JumpTrueCodeGenerator
    {
        public static void GenerateCode(JumpTrueEntry entry, Assembler assembler)
        {
            // Pop i4 from stack and jump if non zero
            assembler.Pop(R16.HL);
            assembler.Pop(R16.HL);
            assembler.Ld(R8.A, 0);
            assembler.Add(R8.A, R8.L);
            assembler.Jp(Condition.NonZero, entry.TargetLabel);
        }
    }
}
