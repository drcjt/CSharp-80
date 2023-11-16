namespace ILCompiler.Compiler.EvaluationStack
{
    public class BinaryOperator : StackEntry
    {
        public StackEntry Op1 { get; }
        public StackEntry Op2 { get; }
        public bool IsComparison { get; }
        public Operation Operation { get; set; }

        public BinaryOperator(Operation operation, bool isComparison, StackEntry op1, StackEntry op2, VarType type) : base(type, type.GetTypeSize())
        {
            Operation = operation;
            IsComparison = isComparison;
            Op1 = op1;
            Op2 = op2;
        }

        public override StackEntry Duplicate()
        {
            return new BinaryOperator(Operation, IsComparison, Op1.Duplicate(), Op2.Duplicate(), Type);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
