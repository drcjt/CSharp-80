using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.Lowerings
{
    internal class JumpTrueLowering : ILowering<JumpTrueEntry>
    {
        public StackEntry? Lower(JumpTrueEntry entry)
        {
            var condition = entry.Condition;

            if (condition is BinaryOperator binOp && binOp.IsComparison &&
                binOp.Op1.IsIntegralConstant(0) && (binOp.Operation == Operation.Eq || binOp.Operation == Operation.Ne_Un))
            {
                    binOp.Op1.Contained = true;

                    // Remove condition from LIR 
                    // op2 -> op1 -> compare -> jumptrue
                    // op2 -> op1 -> jumptrue
                    binOp.Op1.Next = binOp.Next;
            }

            return null;
        }
    }
}
