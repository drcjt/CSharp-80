namespace ILCompiler.Compiler.EvaluationStack
{
    public class PutArgTypeEntry : StackEntry
    {
        public VarType ArgType { get; }
        public StackEntry Op1 { get; }

        public PutArgTypeEntry(VarType argType, StackEntry op1) : base(op1.Kind, op1.ExactSize)
        {
            ArgType = argType;
            Op1 = op1;
        }

        public override StackEntry Duplicate()
        {
            return new PutArgTypeEntry(ArgType, Op1.Duplicate());
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
