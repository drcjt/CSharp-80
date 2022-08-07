using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class IndirectEntry : StackEntry
    {

        public StackEntry Op1 { get; }

        public uint Offset { get; }

        public int DesiredSize { get; }

        public bool SourceInHeap { get; set; }

        public IndirectEntry(StackEntry op1, StackValueKind kind, int? exactSize, int desiredSize = 4, uint offset = 0) : base(kind, exactSize)
        {
            Op1 = op1;
            Offset = offset;
            DesiredSize = desiredSize;
        }

        public override StackEntry Duplicate()
        {
            return new IndirectEntry(Op1.Duplicate(), Kind, ExactSize, DesiredSize, Offset) { SourceInHeap = this.SourceInHeap };
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
