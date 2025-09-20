using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.Inlining
{
    public class InlineResult
    {
        public InlinePolicy Policy { get; init; } = new InlinePolicy();
        public required CallEntry InlineCall { get; set; }

        public bool IsDecided => Policy.Decision.IsDecided();
        public bool IsFailure => Policy.Decision.IsFailure();

        public InlineObservation? Observation => Policy.Observation;

        public void NoteSuccess() => Policy.NoteSuccess();
        public void NoteFatal(InlineObservation observation) => Policy.NoteFatal(observation);

        public void NoteBool(InlineObservation observation, bool value) => Policy.NoteBool(observation, value);
        public void NoteInt(InlineObservation observation, int value) => Policy.NoteInt(observation, value);
    }
}
