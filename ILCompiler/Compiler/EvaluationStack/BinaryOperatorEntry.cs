using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public enum BinaryOp
    {
        ADD,
        SUB,
        MUL,
        EQ,
        NE,
        LT,
        LE,
        GT,
        GE
    }

    public class BinaryOperator : StackEntry
    {
        public BinaryOp Op { get; }
        public StackEntry Op1 { get; }
        public StackEntry Op2 { get; }

        public BinaryOperator(BinaryOp op, StackEntry op1, StackEntry op2, StackValueKind kind) : base(kind)
        {
            Op = op;
            Op1 = op1;
            Op2 = op2;
        }
    }
}
