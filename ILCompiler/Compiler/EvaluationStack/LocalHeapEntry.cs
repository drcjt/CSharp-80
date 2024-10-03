using ILCompiler.Compiler.LinearIR;
using System.Diagnostics.CodeAnalysis;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class LocalHeapEntry : StackEntry
    {
        public StackEntry Op1 { get; set; }

        public LocalHeapEntry(StackEntry op1) : base(VarType.Ptr, VarType.Ptr.GetTypeSize())
        {
            Op1 = op1;
        }

        public override void Accept(IStackEntryVisitor visitor) => visitor.Visit(this);

        public override StackEntry Duplicate()
        {
            return new LocalHeapEntry(Op1);
        }

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
