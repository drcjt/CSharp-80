namespace ILCompiler.Compiler.EvaluationStack
{
    public class CallEntry : StackEntry
    {
        public string TargetMethod { get; }
        public IList<StackEntry> Arguments { get; }

        public bool IsInternalCall { get; }

        public CallEntry(string targetMethod, IList<StackEntry> arguments, VarType returnType, int? returnSize, bool isInternalCall = false) : base(returnType, returnSize)
        {
            TargetMethod = targetMethod;
            Arguments = arguments;
            IsInternalCall = isInternalCall;
        }

        public override StackEntry Duplicate()
        {
            return new CallEntry(TargetMethod, Arguments, Type, ExactSize, IsInternalCall);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
