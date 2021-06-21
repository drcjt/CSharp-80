namespace ILCompiler.Compiler.EvaluationStack
{
    public class StoreLocalVariableEntry : StackEntry
    {
        public StackEntry Op1 { get; }

        public int LocalNumber { get; }

        public StoreLocalVariableEntry(int localNumber, StackEntry op1) : base(op1.Kind)
        {
            LocalNumber = localNumber;
            Op1 = op1;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
