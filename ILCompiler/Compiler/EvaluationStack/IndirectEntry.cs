using ILCompiler.Common.TypeSystem;
using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class IndirectEntry : StackEntry
    {
        public StackEntry Op1 { get; }
        public WellKnownType TargetType { get; }

        public IndirectEntry(StackEntry op1, StackValueKind kind, WellKnownType targetType) : base(kind, targetType.GetWellKnownTypeSize())
        {
            Operation = Operation.Indirect;
            Op1 = op1;
            TargetType = targetType;
        }

        public override StackEntry Duplicate()
        {
            return new IndirectEntry(Op1.Duplicate(), Kind, TargetType);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
