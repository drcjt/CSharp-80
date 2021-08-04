using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class JumpEntry : StackEntry
    {
        public string TargetLabel { get; }

        public JumpEntry(string targetLabel) : base(StackValueKind.Unknown)
        {
            Operation = Operation.Jump;
            TargetLabel = targetLabel;
        }

        public override StackEntry Duplicate()
        {
            return new JumpEntry(TargetLabel);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
