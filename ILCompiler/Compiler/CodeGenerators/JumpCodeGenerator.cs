using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    public class JumpCodeGenerator
    {
        public static void GenerateCode(JumpEntry entry, Assembler assembler)
        {
            assembler.Jp(entry.TargetLabel);
        }
    }
}
