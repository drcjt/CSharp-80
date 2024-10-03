using ILCompiler.Compiler.LinearIR;
using System.Diagnostics.CodeAnalysis;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class IndirectEntry : StackEntry
    {

        public StackEntry Op1 { get; set; }

        public uint Offset { get; }

        public IndirectEntry(StackEntry op1, VarType type, int? exactSize, uint offset = 0) : base(type, exactSize)
        {
            Op1 = op1;
            Offset = offset;
        }

        public override StackEntry Duplicate()
        {
            return new IndirectEntry(Op1.Duplicate(), Type, ExactSize, Offset);
        }

        public override void Accept(IStackEntryVisitor visitor) => visitor.Visit(this);

        public override bool TryGetUse(StackEntry operand, [NotNullWhen(true)] out Edge<StackEntry>? edge)
        {
            if (operand == Op1)
            {
                edge = new Edge<StackEntry>(() => Op1, x => Op1 = x);
                return true;

            }
            return base.TryGetUse(operand, out edge);
        }
    }
}
