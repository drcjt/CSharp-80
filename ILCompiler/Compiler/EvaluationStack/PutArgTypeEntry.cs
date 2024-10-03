using ILCompiler.Compiler.LinearIR;
using System.Diagnostics.CodeAnalysis;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class PutArgTypeEntry : StackEntry
    {
        public VarType ArgType { get; }
        public StackEntry Op1 { get; set; }

        public PutArgTypeEntry(VarType argType, StackEntry op1) : base(op1.Type, op1.ExactSize)
        {
            ArgType = argType;
            Op1 = op1;
        }

        public override StackEntry Duplicate()
        {
            return new PutArgTypeEntry(ArgType, Op1.Duplicate());
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
