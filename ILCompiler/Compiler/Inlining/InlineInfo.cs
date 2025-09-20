using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.TypeSystem.Common;

namespace ILCompiler.Compiler.Inlining
{
    public record InlineInfo
    {
        public required CallEntry InlineCall { get; set; }
        public InlineContext? InlineContext { get; set; }

        public InlineResult? InlineResult { get; set; }
        public InlineArgumentInfo[] InlineArgumentInfos { get; set; } = [];
        public InlineLocalVariableInfo[] LocalVariableInfos { get; set; } = [];
        public required LocalVariableTable InlineLocalVariableTable { get; set; }

        public int? InlineeReturnSpillTempNumber { get; set; } = null;

        public InlineCandidateInfo? InlineCandidateInfo { get; set; } = null;
        public MethodDesc? StaticConstructorMethod { get; set; } = null;
    }
}
