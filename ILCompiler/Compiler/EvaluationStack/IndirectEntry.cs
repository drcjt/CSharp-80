using ILCompiler.Common.TypeSystem;
using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class IndirectEntry : StackEntry
    {

        public StackEntry Op1 { get; }

        public uint Offset { get; }

        public IndirectEntry(StackEntry op1, StackValueKind kind, int? exactSize, uint offset = 0) : base(kind, exactSize)
        {
            Op1 = op1;
            Offset = offset;
        }

        public override StackEntry Duplicate()
        {
            return new IndirectEntry(Op1.Duplicate(), Kind, ExactSize, Offset);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
