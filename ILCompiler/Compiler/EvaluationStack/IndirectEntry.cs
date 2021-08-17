using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class IndirectEntry : StackEntry
    {
        public StackEntry Op1 { get; }

        public IndirectEntry(StackEntry op1, StackValueKind kind) : base(kind)
        {
            Operation = Operation.Indirect;
            Op1 = op1;
        }

        public override StackEntry Duplicate()
        {
            return new IndirectEntry(Op1.Duplicate(), Kind);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
