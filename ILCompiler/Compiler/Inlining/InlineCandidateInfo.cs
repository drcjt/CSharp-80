using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.Inlining
{
    public record InlineCandidateInfo
    {
        public ReturnExpressionEntry? ReturnExpressionEntry { get; set; } = null;
        public InlineContext? InlinersContext { get; set; } = null;
    }
}
