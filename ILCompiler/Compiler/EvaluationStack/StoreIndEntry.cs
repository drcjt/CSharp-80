namespace ILCompiler.Compiler.EvaluationStack
{
    // STOREIND
    public class StoreIndEntry : StackEntry
    {
        public StackEntry Addr { get; }
        public StackEntry Op1 { get; }

        public StoreIndEntry(StackEntry addr, StackEntry op1) : base(addr.Kind)
        {
            Addr = addr;
            Op1 = op1;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
