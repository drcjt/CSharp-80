using ILCompiler.Common.TypeSystem;
using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class CastEntry : StackEntry
    {
        public WellKnownType DesiredType { get; }
        public StackEntry Op1 { get; }

        public CastEntry(WellKnownType desiredType, StackEntry op1, StackValueKind kind) : base(kind, op1.ExactSize)
        {
            DesiredType = desiredType;
            Op1 = op1;
        }

        public override StackEntry Duplicate()
        {
            return new CastEntry(DesiredType, Op1.Duplicate(), Op1.Kind);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
