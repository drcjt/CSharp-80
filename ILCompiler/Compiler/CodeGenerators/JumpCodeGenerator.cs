using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class JumpCodeGenerator : ICodeGenerator<JumpEntry>
    {
        public void GenerateCode(JumpEntry entry, CodeGeneratorContext context)
        {
            context.Emitter.Jp(entry.TargetLabel);
        }
    }
}
