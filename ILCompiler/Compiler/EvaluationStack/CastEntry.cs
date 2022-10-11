using ILCompiler.Common.TypeSystem;
using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class CastEntry : StackEntry
    {
        public WellKnownType DesiredType { get; }

        public VarType DesiredType2 { get; set; }
        public StackEntry Op1 { get; }

        public CastEntry(WellKnownType desiredType, StackEntry op1, VarType type) : base(type, op1.ExactSize)
        {
            DesiredType = desiredType;
            Op1 = op1;
        }

        public override StackEntry Duplicate()
        {
            var duplicate = new CastEntry(DesiredType, Op1.Duplicate(), Type);
            duplicate.DesiredType2 = DesiredType2;
            return duplicate;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
