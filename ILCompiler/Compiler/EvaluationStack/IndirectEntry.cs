namespace ILCompiler.Compiler.EvaluationStack
{
    public class IndirectEntry : StackEntry
    {

        public StackEntry Op1 { get; }

        public uint Offset { get; }

        public int DesiredSize { get; }

        public IndirectEntry(StackEntry op1, VarType type, int? exactSize, int desiredSize = 4, uint offset = 0) : base(type, exactSize)
        {
            Op1 = op1;
            Offset = offset;
            DesiredSize = desiredSize;
        }

        public override StackEntry Duplicate()
        {
            return new IndirectEntry(Op1.Duplicate(), Type, ExactSize, DesiredSize, Offset);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
