using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class CallCodeGenerator : ICodeGenerator<CallEntry>
    {
        public void GenerateCode(CallEntry entry, CodeGeneratorContext context)
        {
            context.Assembler.Call(entry.TargetMethod);
        }
    }
}
