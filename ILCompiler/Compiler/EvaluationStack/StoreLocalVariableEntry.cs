using System.Diagnostics.CodeAnalysis;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class StoreLocalVariableEntry : StackEntry, ILocalVariable, IStoreEntry
    {
        public StackEntry Op1 { get; set; }

        public int LocalNumber { get; }
        public int SsaNumber { get; set; }

        public bool IsParameter { get; }

        public StoreLocalVariableEntry(int localNumber, bool parameter, StackEntry op1, VarType type = VarType.Void, int? exactSize = null) : base(type, exactSize)
        {
            LocalNumber = localNumber;
            IsParameter = parameter;
            Op1 = op1;
        }

        public override StackEntry Duplicate()
        {
            return new StoreLocalVariableEntry(LocalNumber, IsParameter, Op1.Duplicate(), Type, ExactSize);
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
