namespace ILCompiler.Compiler.EvaluationStack
{
    public class CommaEntry : StackEntry
    {
        public StackEntry Op1 { get; }
        public StackEntry Op2 { get; }

        public CommaEntry(StackEntry op1, StackEntry op2) : base(op2.Type, op2.Type.GetTypeSize())
        {
            Op1 = op1;
            Op2 = op2;
        }

        public override StackEntry Duplicate()
        {
            return new CommaEntry(Op1.Duplicate(), Op2.Duplicate());
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
