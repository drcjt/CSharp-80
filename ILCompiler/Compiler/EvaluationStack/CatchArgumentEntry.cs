namespace ILCompiler.Compiler.EvaluationStack
{
    public class CatchArgumentEntry : StackEntry
    {
        public CatchArgumentEntry() : base(VarType.Ref, VarType.Ref.GetTypeSize())
        {
        }

        public override StackEntry Duplicate()
        {
            return new CatchArgumentEntry();
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
