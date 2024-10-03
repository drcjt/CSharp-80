using ILCompiler.Compiler.LinearIR;
using System.Diagnostics.CodeAnalysis;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class StoreIndEntry : StackEntry
    {
        public StackEntry Addr { get; set; }
        public StackEntry Op1 { get; set; }
        public uint FieldOffset { get; }

        public StoreIndEntry(StackEntry addr, StackEntry op1, VarType type, uint fieldOffset = 0, int? size = 4) : base(type, size)
        {
            Addr = addr;
            Op1 = op1;
            FieldOffset = fieldOffset;
        }

        public override StackEntry Duplicate()
        {
            return new StoreIndEntry(Addr, Op1.Duplicate(), Type, FieldOffset, ExactSize);
        }

        public override void Accept(IStackEntryVisitor visitor) => visitor.Visit(this);

        public override bool TryGetUse(StackEntry operand, [NotNullWhen(true)] out Edge<StackEntry>? edge)
        {
            if (operand == Addr)
            {
                edge = new Edge<StackEntry>(() => Addr, x => Addr = x);
                return true;
            }
            if (operand == Op1)
            {
                edge = new Edge<StackEntry>(() => Op1, x => Op1 = x);
                return true;
            }
            return base.TryGetUse(operand, out edge);
        }
    }
}
