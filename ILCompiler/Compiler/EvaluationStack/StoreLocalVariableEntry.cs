namespace ILCompiler.Compiler.EvaluationStack
{
    public class StoreLocalVariableEntry : StackEntry
    {
        public StackEntry Op1 { get; }

        public int LocalNumber { get; }

        public StoreLocalVariableEntry(int localNumber, StackEntry op1) : base(op1.Kind)
        {
            Operation = Operation.StoreLocalVariable;
            LocalNumber = localNumber;
            Op1 = op1;
        }

        public override StackEntry Duplicate()
        {
            return new StoreLocalVariableEntry(LocalNumber, Op1.Duplicate());
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
