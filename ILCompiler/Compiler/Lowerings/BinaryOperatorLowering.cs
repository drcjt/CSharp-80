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
                    return LowerMul(entry);

                case Operation.Div_Un:
                    return LowerUnsignedDiv(entry);

                case Operation.Div:
                    return LowerSignedDiv(entry);

                case Operation.Add:
                    LowerAdd(entry);
                    break;

                case Operation.Sub: 
                    LowerSub(entry);
                    break;

                case Operation.Lsh:
                case Operation.Rsh:
                    LowerShift(entry);
                    break;
            }

            return null;
        }

        private static StackEntry? LowerMul(BinaryOperator mul)
        {
            // Convert multiplication by power of 2 into a left shift
            if (mul.Op2 is Int32ConstantEntry multiplier)
            {
                if (IsPow2(multiplier.Value))
                {
                    mul.Operation = Operation.Lsh;
                    multiplier.Value = GetLog2(multiplier.Value);

                    return mul;
                }
            }

            return null;
        }

        private static StackEntry? LowerUnsignedDiv(BinaryOperator div)
        {
            if (div.Op2 is Int32ConstantEntry divisor)
            {
                if (IsPow2(divisor.Value))
                {
                    div.Operation = Operation.Rsh;
                    divisor.Value = GetLog2(divisor.Value);

                    return div;
                }
            }

            return null;
        }

        private static StackEntry? LowerSignedDiv(BinaryOperator div)
        {
            if (div.Op2 is Int32ConstantEntry divisor)
            {
                if (divisor.Value == int.MinValue)
                {
                    // x / int.MinValue, becomes, x == int.MinValue
                    div.Operation = Operation.Eq;
                    return div;
                }

                if (IsPow2(divisor.Value))
                {
                    // TODO: Implement proper signed div for power of 2
                    // using right shift here.
                }
            }

            return null;
        }

        private static void LowerAdd(BinaryOperator add)
        {
            ContainCheckBinary(add);
        }

        private static void LowerSub(BinaryOperator sub)
        {
            ContainCheckBinary(sub);
        }

        private static void LowerShift(BinaryOperator lsh)
        {
            if (lsh.Op2.IsIntCnsOrI())
            {
                var shiftBy = lsh.Op2.As<Int32ConstantEntry>().Value;

                if (shiftBy == 1 || shiftBy == 8 || shiftBy == 16 || shiftBy > 31)
                {
                    ContainCheckBinary(lsh);
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

        private static void ContainCheckBinary(BinaryOperator node)
        {
            var op2 = node.Op2;

            bool directlyEncodable = false;
            StackEntry? operand = null;

            if (IsContainableImmediate(node, op2))
            {
                directlyEncodable = true;
                operand = op2;
            }
            else
            {
                // Todo: more containment stuff
            }

            if (directlyEncodable && operand != null)
            {
                MakeSrcContained(node, operand);
            }
        }

        private static bool IsContainableImmediate(StackEntry parent, StackEntry child) => child.IsIntCnsOrI();

        private static void MakeSrcContained(StackEntry parent, StackEntry child)
        {
            child.Contained = true;

            // TODO: check is containable memory op
        }
    }
}
