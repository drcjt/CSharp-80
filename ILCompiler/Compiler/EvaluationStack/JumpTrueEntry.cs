using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class JumpTrueEntry : StackEntry
    {
        public StackEntry Condition { get; }
        public string TargetLabel { get; }
        public JumpTrueEntry(string targetLabel, StackEntry condition) : base(StackValueKind.Unknown)
        {
            TargetLabel = targetLabel;
            Condition = condition;
        }

        public override StackEntry Duplicate()
        {
            return new JumpTrueEntry(TargetLabel, Condition.Duplicate());
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
