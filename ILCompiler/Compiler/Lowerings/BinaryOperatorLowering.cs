using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.Lowerings
{
    internal class BinaryOperatorLowering : ILowering<BinaryOperator>
    {
        public StackEntry? Lower(BinaryOperator entry)
        {
            // TODO: Check whether a binary op's operands should be contained

            switch (entry.Operation)
            {
                case Operation.Mul:
                    LowerMul(entry);
                    break;

                case Operation.Div:
                    LowerDiv(entry);
                    // TODO: Convert division by pow of 2 into right shift
                    break;
            }

            return null;
        }

        private void LowerMul(BinaryOperator mul)
        {
            // Convert multiplication by power of 2 into a left shift
            if (mul.Op2 is Int32ConstantEntry)
            {
                var multiplier = mul.Op2.As<Int32ConstantEntry>();
                if (IsPow2(multiplier.Value))
                {
                    mul.Operation = Operation.Lsh;
                    multiplier.Value = GetLog2(multiplier.Value);
                }
            }
        }

        private void LowerDiv(BinaryOperator div)
        {
            if (div.Op2 is Int32ConstantEntry)
            {
                var divisor = div.Op2.As<Int32ConstantEntry>();
                if (IsPow2(divisor.Value))
                {
                    div.Operation = Operation.Rsh;
                    divisor.Value = GetLog2(divisor.Value);
                }
            }
        }

        private static bool IsPow2(int i)
        {
            return (i > 0 && ((i - 1) & i) == 0);
        }

        private static int GetLog2(int i)
        {
            int r = 0;
            while ((i >>= 1) != 0)
            {
                ++r;
            }
            return r;
        }
    }
}
