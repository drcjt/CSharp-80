namespace ILCompiler.Compiler.EvaluationStack
{
    public class BoundsCheck : StackEntry
    {
        public StackEntry Index { get; }
        public StackEntry ArrayLength { get; }

        public BoundsCheck(StackEntry index, StackEntry arrayLength) : base(VarType.Void)
        {
            Index = index;
            ArrayLength = arrayLength;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override StackEntry Duplicate()
        {
            return new BoundsCheck(Index.Duplicate(), ArrayLength.Duplicate());
        }
    }
}
