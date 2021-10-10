using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class BinaryOperator : StackEntry
    {
        public StackEntry Op1 { get; }
        public StackEntry Op2 { get; }

        public BinaryOperator(Operation operation, StackEntry op1, StackEntry op2, StackValueKind kind) : base(kind, op1.ExactSize)
        {
            Operation = operation;
            Op1 = op1;
            Op2 = op2;
        }

        public override StackEntry Duplicate()
        {
            return new BinaryOperator(Operation, Op1.Duplicate(), Op2.Duplicate(), Kind);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
