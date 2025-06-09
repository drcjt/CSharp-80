using System.Diagnostics.CodeAnalysis;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class BoundsCheck : StackEntry
    {
        public StackEntry Index { get; set; }
        public StackEntry ArrayLength { get; set; }

        public BoundsCheck(StackEntry index, StackEntry arrayLength) : base(VarType.Void)
        {
            Index = index;
            ArrayLength = arrayLength;
        }

        public override void Accept(IStackEntryVisitor visitor) => visitor.Visit(this);

        public override StackEntry Duplicate()
        {
            return new BoundsCheck(Index.Duplicate(), ArrayLength.Duplicate());
        }

        public override bool TryGetUse(StackEntry operand, [NotNullWhen(true)] out Edge<StackEntry>? edge)
        {
            if (Index == operand)
            {
                edge = new Edge<StackEntry>(() => Index, x => Index = x);
                return true;
            }
            return base.TryGetUse(operand, out edge);
        }
    }
}
