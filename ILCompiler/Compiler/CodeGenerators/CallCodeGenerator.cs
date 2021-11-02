using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    public class CallCodeGenerator
    {
        public static void GenerateCode(CallEntry entry, Assembler assembler)
        {
            assembler.Call(entry.TargetMethod);
        }
    }
}
