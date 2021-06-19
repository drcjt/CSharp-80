using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    // JTRUE
    public class JumpTrueEntry : StackEntry
    {
        public StackEntry Condition { get; }
        public JumpTrueEntry(StackEntry condition) : base(StackValueKind.Unknown)
        {
            Condition = condition;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
