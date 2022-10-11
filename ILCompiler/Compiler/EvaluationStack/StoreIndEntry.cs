using ILCompiler.Common.TypeSystem;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class StoreIndEntry : StackEntry
    {
        public StackEntry Addr { get; }
        public StackEntry Op1 { get; }
        public uint FieldOffset { get; }

        public bool TargetInHeap { get; set; }

        public WellKnownType TargetType { get; }

        public StoreIndEntry(StackEntry addr, StackEntry op1, WellKnownType targetType, uint fieldOffset = 0, int? size = 4) : base(VarType.Void, size)
        {
            Addr = addr;
            Op1 = op1;
            FieldOffset = fieldOffset;
            TargetType = targetType;
        }

        public override StackEntry Duplicate()
        {
            return new StoreIndEntry(Addr, Op1.Duplicate(), TargetType, FieldOffset, ExactSize) { TargetInHeap = this.TargetInHeap };
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
