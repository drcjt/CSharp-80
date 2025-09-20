using System.Diagnostics;

namespace ILCompiler.Compiler.Inlining
{
    public class InlinePolicy
    {
        public InlineDecision Decision { get; private set; } = InlineDecision.Undecided;
        public InlineObservation? Observation { get; private set; }

        public int CodeSize { get; private set; }
        public bool? ForceInline { get; private set; } = null;
        public int CallSiteDepth { get; private set; } = 0;

        public void NoteSuccess()
        {
            Debug.Assert(Decision.IsCandidate());
            Decision = InlineDecision.Success;
        }

        public void NoteFatal(InlineObservation observation)
        {
            NoteInternal(observation);
        }

        public void NoteBool(InlineObservation observation, bool value)
        {
            if (observation.name == InlineObservationName.IsForceInline)
            {
                ForceInline = value;
            }
        }

        public void NoteInt(InlineObservation observation, int value)
        {
            const int AlwaysInlineSize = 16;
            const int MaxInlineDepth = 20;

            switch (observation.name)
            {
                case InlineObservationName.ILCodeSize:
                    CodeSize = value;

                    if (ForceInline == true)
                    {
                        SetCandidate(InlineObservation.IsForceInline);
                    }
                    else if (CodeSize < AlwaysInlineSize)
                    {
                        SetCandidate(InlineObservation.BelowAlwaysInlineSize);
                    }
                    else
                    {
                        SetNever(InlineObservation.TooMuchIL);
                    }
                    break;

                case InlineObservationName.Depth:
                    CallSiteDepth = value;
                    if (CallSiteDepth > MaxInlineDepth)
                    {
                        SetNever(InlineObservation.IsTooDeep);
                    }
                    break;

                default:
                    break;
            }
        }

        private void NoteInternal(InlineObservation observation)
        {
            if (observation.target == InlineTarget.Callee)
            {
                SetNever(observation);
            }
            else
            {
                SetFailure(observation);
            }
        }

        private void SetFailure(InlineObservation observation)
        {
            Decision = InlineDecision.Failure;
            Observation = observation;
        }

        private void SetNever(InlineObservation observation)
        {
            Decision = InlineDecision.Never;
            Observation = observation;
        }

        private void SetCandidate(InlineObservation observation)
        {
            if (Decision == InlineDecision.Undecided)
            {
                Decision = InlineDecision.Candidate;
                Observation = observation;
            }
        }
    }
}
