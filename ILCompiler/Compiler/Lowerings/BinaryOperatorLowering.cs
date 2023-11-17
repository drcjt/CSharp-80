using ILCompiler.Compiler.EvaluationStack;
using System.Diagnostics;

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
                    break;

                case Operation.Add:
                    LowerAdd(entry);
                    break;

                case Operation.Sub: 
                    LowerSub(entry);
                    break;
            }

            return null;
        }

        private static void LowerMul(BinaryOperator mul)
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

        private static void LowerDiv(BinaryOperator div)
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

        private static void LowerAdd(BinaryOperator add)
        {
            ContainCheckBinary(add);
        }

        private static void LowerSub(BinaryOperator sub)
        {
            ContainCheckBinary(sub);
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

        private static bool IsContainableImmediate(StackEntry parent, StackEntry child) => child.IsIntCns();

        private static void MakeSrcContained(StackEntry parent, StackEntry child)
        {
            child.Contained = true;

            // TODO: check is containable memory op
        }
    }
}
