namespace ILCompiler.Compiler.EvaluationStack
{
    // STOREIND
    public class IndEntry : StackEntry
    {
        public StackEntry Addr { get; }

        public IndEntry(StackEntry addr) : base(addr.Kind)
        {
            Addr = addr;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
