using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class UnaryOperator : StackEntry
    {
        public StackEntry Op1 { get; }

        public UnaryOperator(Operation operation, StackEntry op1) : base(op1.Kind)
        {
            Operation = operation;
            Op1 = op1;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }

    }
}
