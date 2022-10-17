namespace ILCompiler.Compiler.EvaluationStack
{
    public class StoreIndEntry : StackEntry
    {
        public StackEntry Addr { get; }
        public StackEntry Op1 { get; }
        public uint FieldOffset { get; }

        public StoreIndEntry(StackEntry addr, StackEntry op1, uint fieldOffset = 0, int? size = 4) : base(VarType.Void, size)
        {
            Addr = addr;
            Op1 = op1;
            FieldOffset = fieldOffset;
        }

        public override StackEntry Duplicate()
        {
            return new StoreIndEntry(Addr, Op1.Duplicate(), FieldOffset, ExactSize);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
