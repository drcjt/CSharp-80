using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.LinearIR;

namespace ILCompiler.Compiler.Lowerings
{
    internal class BinaryOperatorLowering : ILowering<BinaryOperator>
    {
        public StackEntry? Lower(BinaryOperator entry, BasicBlock block, LocalVariableTable locals)
        {
            // TODO: Check whether a binary op's operands should be contained

            switch (entry.Operation)
            {
                case Operation.Mul:
                    return LowerMul(entry);

                case Operation.Div_Un:
                    return LowerUnsignedDiv(entry);

                case Operation.Div:
                    return LowerSignedDiv(entry, block, locals);

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

        private static StackEntry? LowerSignedDiv(BinaryOperator div, BasicBlock block, LocalVariableTable locals)
        {
            if (div.Op2 is Int32ConstantEntry divisor)
            {
                if (divisor.Value == int.MinValue)
                {
                    // x / int.MinValue, becomes, x == int.MinValue
                    div.Operation = Operation.Eq;
                    return div;
                }

                var absDivisor = Math.Abs(divisor.Value);
                if (IsPow2(absDivisor))
                {
                    var blockRange = block;
                    if (!blockRange.TryGetUse(div, out Use? use))
                    {
                        return null;        
                    }

                    Use opDividend = new Use(blockRange, new Edge<StackEntry>(() => div.Op1, x => div.Op1 = x), div);
                    var dividend = opDividend.ReplaceWithLclVar(locals);

                    var adjustment = new BinaryOperator(Operation.Rsh, false, dividend, new Int32ConstantEntry(31), VarType.Int);
                    adjustment = new BinaryOperator(Operation.And, false, adjustment, new Int32ConstantEntry(absDivisor - 1), VarType.Int);
                    var adjustedDividend = new BinaryOperator(Operation.Add, false, adjustment,
                        new LocalVariableEntry(dividend.As<LocalVariableEntry>().LocalNumber, dividend.As<LocalVariableEntry>().Type, dividend.As<LocalVariableEntry>().ExactSize), VarType.Int);

                    // Implement the division by right shifting the adjusted dividend
                    var divisorValue = divisor.Value;
                    divisor.Value = GetLog2(absDivisor);

                    StackEntry newDiv = new BinaryOperator(Operation.Rsh, false, adjustedDividend, divisor, VarType.Int);

                    if (divisorValue < 0)
                    {
                        // Negate the result if the divisor is negative
                        newDiv = new UnaryOperator(Operation.Neg, newDiv);
                    }

                    // Remove nodes that have been reused from the linear order
                    // as these will be re-added when the tree is re-sequenced
                    blockRange.Remove(divisor);
                    blockRange.Remove(dividend);

                    // linearise the new div tree and insert before the original
                    InsertTreeBefore(div, newDiv, blockRange);
                    blockRange.Remove(div);

                    // Replace the original div node with the newdiv tree
                    use.ReplaceWith(newDiv);

                    return newDiv.Next;
                }
            }

            return null;
        }

        private static void InsertTreeBefore(StackEntry insertionPoint, StackEntry tree, LinearIR.Range blockRange)
        {
            var range = LIR.SeqTree(tree);
            blockRange.InsertBefore(insertionPoint, range);
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

        private static bool IsPow2(int i) => (i > 0 && ((i - 1) & i) == 0);

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
