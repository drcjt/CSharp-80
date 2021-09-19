using dnlib.DotNet;
using ILCompiler.Common.TypeSystem;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class StoreIndEntry : StackEntry
    {
        public StackEntry Addr { get; }
        public StackEntry Op1 { get; }
        public uint FieldOffset => FieldDef?.FieldOffset ?? 0;
        public int FieldSize => FieldDef?.FieldType.GetExactSize(false) ?? 4;

        public FieldDef FieldDef { get; }

        public WellKnownType TargetType { get; }

        public StoreIndEntry(StackEntry addr, StackEntry op1, WellKnownType targetType, FieldDef fieldDef = null) : base(addr.Kind)
        {
            Operation = Operation.StoreIndirect;
            Addr = addr;
            Op1 = op1;
            FieldDef = fieldDef;
            TargetType = targetType;
        }

        public override StackEntry Duplicate()
        {
            return new StoreIndEntry(Addr, Op1.Duplicate(), TargetType, FieldDef);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
