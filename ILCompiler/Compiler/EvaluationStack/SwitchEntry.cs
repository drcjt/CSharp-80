using ILCompiler.Compiler.LinearIR;
using System.Diagnostics.CodeAnalysis;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class SwitchEntry : StackEntry
    {
        public StackEntry Op1 { get; set; }
        public IList<string> JumpTable { get; }

        public SwitchEntry(StackEntry op1, IList<string> jumpTable) : base(VarType.Void)
        {
            Op1 = op1;
            JumpTable = jumpTable;
        }

        public override StackEntry Duplicate()
        {
            return new SwitchEntry(Op1, JumpTable);
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
