using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class BinaryOperator : StackEntry
    {
        public StackEntry Op1 { get; }
        public StackEntry Op2 { get; }

        public BinaryOperator(Operation operation, StackEntry op1, StackEntry op2, StackValueKind kind) : base(kind)
        {
            Operation = operation;
            Op1 = op1;
            Op2 = op2;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
