using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class IntrinsicEntry : StackEntry
    {
        public string TargetMethod { get; }
        public IList<StackEntry> Arguments { get; }

        public IntrinsicEntry(string targetMethod, IList<StackEntry> arguments, StackValueKind returnKind) : base(returnKind)
        {
            TargetMethod = targetMethod;
            Arguments = arguments;
        }

        public override StackEntry Duplicate()
        {
            return new IntrinsicEntry(TargetMethod, Arguments, Kind);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
