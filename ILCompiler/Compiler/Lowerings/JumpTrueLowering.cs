using System.Diagnostics;
using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.Lowerings
{
    internal class JumpTrueLowering : ILowering<JumpTrueEntry>
    {
        public StackEntry? Lower(JumpTrueEntry entry, BasicBlock block, LocalVariableTable locals)
        {
            var condition = entry.Condition;

            if (condition is BinaryOperator binOp && binOp.IsComparison &&
                binOp.Op1.IsIntCnsOrI() && (binOp.Operation == Operation.Eq || binOp.Operation == Operation.Ne_Un))
            {
                if (binOp.Op2.Contained)
                {
                    // TODO: Should be able to eliminate the entire conditional jump but for now leave the compare in the LIR
                    return null;
                }

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
