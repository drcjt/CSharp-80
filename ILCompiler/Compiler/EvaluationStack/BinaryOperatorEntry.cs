using ILCompiler.Compiler.LinearIR;
using System.Diagnostics.CodeAnalysis;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class BinaryOperator : StackEntry
    {
        public StackEntry Op1 { get; set; } 
        public StackEntry Op2 { get; set; }
        public bool IsComparison { get; }
        public Operation Operation { get; set; }
        public bool ResultUsedInJump { get; set; } = false;

        public BinaryOperator(Operation operation, bool isComparison, StackEntry op1, StackEntry op2, VarType type) : base(type, type.GetTypeSize())
        {
            Operation = operation;
            IsComparison = isComparison;
            Op1 = op1;
            Op2 = op2;
        }

        public override StackEntry Duplicate()
        {
            var clone = new BinaryOperator(Operation, IsComparison, Op1.Duplicate(), Op2.Duplicate(), Type);
            clone.ResultUsedInJump = ResultUsedInJump;
            return clone;
        }

        public override void Accept(IStackEntryVisitor visitor) => visitor.Visit(this);

        public override bool TryGetUse(StackEntry operand, [NotNullWhen(true)] out Edge<StackEntry>? edge)
        {
            if (operand == Op1)
            {
                edge = new Edge<StackEntry>(() => Op1, x => Op1 = x);
                return true;
            }
            if (operand == Op2)
            {
                edge = new Edge<StackEntry>(() => Op2, x => Op2 = x);
                return true;
            }
            return base.TryGetUse(operand, out edge);
        }
    }
}
