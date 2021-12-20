using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.Lowerings
{
    internal class BinaryOperatorLowering : ILowering<BinaryOperator>
    {
        public StackEntry? Lower(BinaryOperator entry)
        {
            // TODO: Check whether a binary op's operands should be contained

            return null;
        }
    }
}
