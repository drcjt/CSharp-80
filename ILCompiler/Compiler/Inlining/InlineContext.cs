namespace ILCompiler.Compiler.Inlining
{
    public class InlineContext
    {
        public InlineContext? Parent { get; set; } = null;

        public InlineObservation? Observation { get; set; }
        public bool Success { get; set; } = false;

        public void SetSucceeded(InlineInfo info)
        {
            Observation = info.InlineResult!.Observation;
            Success = true;
        }

        public void SetFailed(InlineResult result)
        {
            Observation = result.Observation;
            Success = false;
        }
    }
}
