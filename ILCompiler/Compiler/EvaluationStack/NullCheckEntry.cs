namespace ILCompiler.Compiler.EvaluationStack
{
    public class NullCheckEntry : StackEntry
    {
        public StackEntry Op1 { get; }

        public NullCheckEntry(StackEntry op1) : base(op1.Type, op1.Type.GetTypeSize() /*op1.ExactSize */)
        {
            Op1 = op1;
        }

        public override StackEntry Duplicate()
        {
            return new NullCheckEntry(Op1.Duplicate());
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
