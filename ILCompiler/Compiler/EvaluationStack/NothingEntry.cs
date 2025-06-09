namespace ILCompiler.Compiler.EvaluationStack
{
    public class NothingEntry : StackEntry
    {
        public NothingEntry() : base(VarType.Void)
        {
        }

        public override StackEntry Duplicate()
        {
            return new NothingEntry();
        }

        public override void Accept(IStackEntryVisitor visitor) => visitor.Visit(this);
    }
}
