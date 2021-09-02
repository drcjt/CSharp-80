using ILCompiler.Common.TypeSystem;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class StoreIndEntry : StackEntry
    {
        public StackEntry Addr { get; }
        public StackEntry Op1 { get; }
        // TODO: remove nullability on these
        // as if not storing to a field then
        // fieldoffset will be 0 and
        // fieldsize will be 4
        public uint? FieldOffset { get; set; }

        public int? FieldSize { get; }

        public WellKnownType TargetType { get; }

        public StoreIndEntry(StackEntry addr, StackEntry op1, WellKnownType targetType, uint? fieldOffset = null, int? fieldSize = null) : base(addr.Kind)
        {
            Operation = Operation.StoreIndirect;
            Addr = addr;
            Op1 = op1;
            FieldOffset = fieldOffset;
            FieldSize = fieldSize;
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
