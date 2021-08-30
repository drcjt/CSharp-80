using ILCompiler.Common.TypeSystem;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class StoreIndEntry : StackEntry
    {
        public StackEntry Addr { get; }
        public StackEntry Op1 { get; }
        public uint? FieldOffset { get; set; }
        public WellKnownType TargetType { get; }

        public StoreIndEntry(StackEntry addr, StackEntry op1, WellKnownType targetType, uint? fieldOffset = null) : base(addr.Kind)
        {
            Operation = Operation.StoreIndirect;
            Addr = addr;
            Op1 = op1;
            FieldOffset = fieldOffset;
            TargetType = targetType;
        }

        public override StackEntry Duplicate()
        {
            return new StoreIndEntry(Addr, Op1.Duplicate(), TargetType, FieldOffset);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
