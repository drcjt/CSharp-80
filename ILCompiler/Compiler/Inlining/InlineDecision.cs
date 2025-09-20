namespace ILCompiler.Compiler.Inlining
{
    public enum InlineDecision
    {
        Undecided,
        Candidate,
        Success,
        Failure,
        Never,
    }

    public static class InlineDecisionExtensions
    {
        public static bool IsDecided(this InlineDecision decision) => decision switch
        {
            InlineDecision.Undecided => false,
            InlineDecision.Candidate => false,
            _ => true
        };
        public static bool IsFailure(this InlineDecision decision) => decision switch
        {
            InlineDecision.Failure => true,
            InlineDecision.Never => true,
            _ => false
        };

        public static bool IsCandidate(this InlineDecision decision) => !decision.IsFailure();
    }
}
