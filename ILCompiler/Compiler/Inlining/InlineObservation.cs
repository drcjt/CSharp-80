namespace ILCompiler.Compiler.Inlining
{
    public enum InlineTarget
    {
        Caller,
        Callee,
        CallSite,
    }

    public enum InlineObservationName
    {
        ILCodeSize,
        IsForceInline,
        BelowAlwaysInlineSize,

        NoMethodInfo,
        HasNoBody,
        IsNoInline,
        IsStructReturn,
        IsPInvoke,
        IsInternal,

        TooMuchIL,

        DebugCodeGen,

        IsNotDirect,
        IsVirtual,
        IsWithinFilter,
        IsRecursive,
        IsTooDeep,

        Depth,
    }

    public record InlineObservation(InlineObservationName name, InlineTarget target)
    {
        // Callee Information
        public static InlineObservation ILCodeSize { get; } = new(InlineObservationName.ILCodeSize, InlineTarget.Callee);
        public static InlineObservation IsForceInline { get; } = new(InlineObservationName.IsForceInline, InlineTarget.Callee);
        public static InlineObservation BelowAlwaysInlineSize { get; } = new(InlineObservationName.BelowAlwaysInlineSize, InlineTarget.Callee);

        // Callee Fatal
        public static InlineObservation NoMethodInfo { get; } = new(InlineObservationName.NoMethodInfo, InlineTarget.Callee);
        public static InlineObservation HasNoBody { get; } = new(InlineObservationName.HasNoBody, InlineTarget.Callee);
        public static InlineObservation IsNoInline { get; } = new(InlineObservationName.IsNoInline, InlineTarget.Callee);
        public static InlineObservation IsStructReturn { get; } = new(InlineObservationName.IsStructReturn, InlineTarget.Callee);
        public static InlineObservation IsPInvoke { get; } = new(InlineObservationName.IsPInvoke, InlineTarget.Callee);
        public static InlineObservation IsInternal { get; } = new(InlineObservationName.IsInternal, InlineTarget.Callee);

        // Callee Performance
        public static InlineObservation TooMuchIL { get; } = new(InlineObservationName.TooMuchIL, InlineTarget.Callee);


        // Caller Correctness
        public static InlineObservation DebugCodeGen { get; } = new(InlineObservationName.DebugCodeGen, InlineTarget.Caller);

        // Callsite Correctness
        public static InlineObservation IsNotDirect { get; } = new(InlineObservationName.IsNotDirect, InlineTarget.CallSite);
        public static InlineObservation IsVirtual { get; } = new(InlineObservationName.IsVirtual, InlineTarget.CallSite);
        public static InlineObservation IsWithinFilter { get; } = new(InlineObservationName.IsWithinFilter, InlineTarget.CallSite);
        public static InlineObservation IsRecursive { get; } = new(InlineObservationName.IsRecursive, InlineTarget.CallSite);
        public static InlineObservation IsTooDeep { get; } = new(InlineObservationName.IsTooDeep, InlineTarget.CallSite);

        // Callsite Information
        public static InlineObservation Depth { get; } = new(InlineObservationName.Depth, InlineTarget.CallSite);
    }
}
