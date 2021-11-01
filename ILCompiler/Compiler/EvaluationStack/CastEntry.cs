using ILCompiler.Common.TypeSystem;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class CastEntry : StackEntry
    {
        public WellKnownType DesiredType { get; }
        public StackEntry Op1 { get; }

        public CastEntry(WellKnownType desiredType, StackEntry op1) : base(op1.Kind, op1.ExactSize)
        {
            DesiredType = desiredType;
            Op1 = op1;
        }

        public override StackEntry Duplicate()
        {
            return new CastEntry(DesiredType, Op1.Duplicate());
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
