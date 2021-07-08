using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class CastEntry : StackEntry
    {
        public StackValueKind DesiredKind { get; }
        public StackEntry Op1 { get; }

        public CastEntry(StackValueKind desiredKind, StackEntry op1) : base(desiredKind)
        {
            Operation = Operation.Cast;
            DesiredKind = desiredKind;
            Op1 = op1;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
