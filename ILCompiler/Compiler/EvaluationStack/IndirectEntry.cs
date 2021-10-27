using ILCompiler.Common.TypeSystem;
using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class IndirectEntry : StackEntry
    {
        public StackEntry Op1 { get; }

        public IndirectEntry(StackEntry op1, StackValueKind kind, int? exactSize) : base(kind, exactSize)
        {
            Operation = Operation.Indirect;
            Op1 = op1;
        }

        public override StackEntry Duplicate()
        {
            return new IndirectEntry(Op1.Duplicate(), Kind, ExactSize);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
