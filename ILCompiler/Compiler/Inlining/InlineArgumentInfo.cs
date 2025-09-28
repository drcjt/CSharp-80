using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.Inlining
{
    public record InlineArgumentInfo
    {
        public required StackEntry Argument { get; set; }
        public int TempNumber { get; set; }
        public bool HasTemp { get; set; }
        public bool IsInvariant { get; set; }
        public bool IsLocalVariable { get; set; }
        public bool IsUsed { get; set; }
        public bool HasLdargaOp { get; set; }
        public bool HasStargOp { get; set; }
    }
}
