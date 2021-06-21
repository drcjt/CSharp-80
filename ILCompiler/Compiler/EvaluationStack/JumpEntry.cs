using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class JumpEntry : StackEntry
    {
        public string TargetLabel { get; }

        public JumpEntry(string targetLabel) : base(StackValueKind.Unknown)
        {
            TargetLabel = targetLabel;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
