using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.Inlining
{
    public record InlineMethodInfo
    {
        public required CallEntry Call { get; set; }
        public required Statement Statement { get; set; }
        public required int StatementIndex { get; set; }
        public required BasicBlock Block { get; set; }
        public required LocalVariableTable Locals { get; set; }
    }
}
