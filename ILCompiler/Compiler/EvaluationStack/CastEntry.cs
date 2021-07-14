using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class CastEntry : StackEntry
    {
        public StackValueKind DesiredKind { get; }
        public StackEntry Op1 { get; }

        public bool Unsigned { get;  }

        public CastEntry(StackValueKind desiredKind, bool unsigned, StackEntry op1) : base(desiredKind)
        {
            Operation = Operation.Cast;
            DesiredKind = desiredKind;
            Op1 = op1;
            Unsigned = unsigned;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
