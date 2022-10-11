namespace ILCompiler.Compiler.EvaluationStack
{
    public class UnaryOperator : StackEntry
    {
        public StackEntry Op1 { get; }
        public Operation Operation { get; set; }

        public UnaryOperator(Operation operation, StackEntry op1) : base(op1.Type, op1.ExactSize)
        {
            Operation = operation;
            Op1 = op1;
        }

        public override StackEntry Duplicate()
        {
            return new UnaryOperator(Operation, Op1.Duplicate());
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
