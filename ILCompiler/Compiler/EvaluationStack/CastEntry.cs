namespace ILCompiler.Compiler.EvaluationStack
{
    public class CastEntry : StackEntry
    {
        public StackEntry Op1 { get; }

        public CastEntry(StackEntry op1, VarType type) : base(type, type.GetTypeSize() /*op1.ExactSize */)
        {
            Op1 = op1;
        }

        public override StackEntry Duplicate()
        {
            return new CastEntry(Op1.Duplicate(), Type);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
