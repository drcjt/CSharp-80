using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System.Diagnostics;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class CodeFolder
    {
        private readonly IConfiguration _configuration;
        public CodeFolder(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public StackEntry FoldExpression(StackEntry tree)
        {
            if (!_configuration.Optimize)
                return tree;

            if (tree is CastEntry castOperator && castOperator.Op1.IsIntCnsOrI())
            {
                return FoldConstantExpression(tree);
            }

            if (tree is BinaryOperator binaryOperator)
            {
                if (binaryOperator.Op1.IsIntCnsOrI() && binaryOperator.Op2.IsIntCnsOrI())
                {
                    return FoldConstantExpression(tree);
                }

                if (binaryOperator.Op1.IsIntCnsOrI() || binaryOperator.Op2.IsIntCnsOrI())
                {
                    return FoldBinaryOpWithOneConstantOperand(binaryOperator);
                }
            }

            return tree;
        }

        private static StackEntry FoldBinaryOpWithOneConstantOperand(BinaryOperator tree)
        {
            StackEntry op;
            StackEntry constant;
            if (tree.Op1.IsIntCnsOrI())
            {
                op = tree.Op2;
                constant = tree.Op1;
            }
            else if (tree.Op2.IsIntCnsOrI())
            {
                op = tree.Op1;
                constant = tree.Op2;
            }
            else
            {
                return tree;
            }

            var value = constant.GetIntConstant();

            if (tree.Operation == Operation.Add && value == 0)
            {
                return op;
            }
            if (tree.Operation == Operation.Mul && value == 1)
            {
                return op;
            }
            if (tree.Operation == Operation.Div && value == 1)
            {
                return op;
            }
            if (tree.Operation == Operation.Sub && tree.Op1 == op && value == 0)
            {
                // Subtracting zero from something is a no-op
                return op;
            }

            return tree;
        }

        private static StackEntry FoldBinaryOperatorWithConstantIntOperands(BinaryOperator tree)
        {
            int i1 = tree.Op1.GetIntConstant();
            int i2 = tree.Op2.GetIntConstant();
            switch (tree.Operation)
            {
                case Operation.Eq:
                    i1 = i1 == i2 ? 1 : 0;
                    break;

                case Operation.Ne_Un:
                    i1 = (uint)i1 != (uint)i2 ? 1 : 0;
                    break;

                case Operation.Gt:
                    i1 = i1 > i2 ? 1 : 0;
                    break;

                case Operation.Gt_Un:
                    i1 = (uint)i1 > (uint)i2 ? 1 : 0;
                    break;

                case Operation.Ge:
                    i1 = i1 >= i2 ? 1 : 0;
                    break;

                case Operation.Ge_Un:
                    i1 = (uint)i1 >= (uint)i2 ? 1 : 0;
                    break;

                case Operation.Lt:
                    i1 = i1 < i2 ? 1 : 0;
                    break;

                case Operation.Lt_Un:
                    i1 = (uint)i1 < (uint)i2 ? 1 : 0;
                    break;

                case Operation.Le:
                    i1 = i1 <= i2 ? 1 : 0;
                    break;

                case Operation.Le_Un:
                    i1 = (uint)i1 <= (uint)i2 ? 1 : 0;
                    break;

                case Operation.Add:
                    i1 = i1 + i2;
                    break;

                case Operation.Sub:
                    i1 = i1 - i2;
                    break;

                case Operation.Mul:
                    i1 = i1 * i2;
                    break;

                case Operation.Div:
                    if (i2 == 0)
                    {
                        // Division by zero, return the original tree which will throw division by zero exception
                        return tree;
                    }
                    i1 = i1 / i2;
                    break;

                case Operation.Div_Un:
                    if (i2 == 0)
                    {
                        // Division by zero, return the original tree which will throw division by zero exception
                        return tree;
                    }
                    i1 = (int)((uint)i1 / (uint)i2);
                    break;

                case Operation.Rem:
                    if (i2 == 0)
                    {
                        // Division by zero, return the original tree which will throw division by zero exception
                        return tree;
                    }
                    i1 = i1 % i2;
                    break;

                case Operation.Rem_Un:
                    if (i2 == 0)
                    {
                        // Division by zero, return the original tree which will throw division by zero exception
                        return tree;
                    }
                    i1 = (int)((uint)i1 % (uint)i2);
                    break;

                case Operation.Lsh:
                    i1 <<= (i2 & 0x1F);
                    break;

                case Operation.Rsh:
                    i1 >>= (i2 & 0x1F);
                    break;

                case Operation.Or:
                    i1 |= i2;
                    break;

                case Operation.And:
                    i1 &= i2;
                    break;

                case Operation.Xor:
                    i1 ^= i2;
                    break;

                default:
                    return tree;
            }

            return new Int32ConstantEntry(i1);
        }

        private static StackEntry FoldBinaryOperatorWithConstantPtrOperands(BinaryOperator tree)
        {
            short i1 = (short)tree.Op1.GetIntConstant();
            short i2 = (short)tree.Op2.GetIntConstant();
            switch (tree.Operation)
            {
                case Operation.Ne_Un:
                    i1 = i1 != i2 ? (short)1 : (short)0;
                    break;

                case Operation.Gt:
                    i1 = i1 > i2 ? (short)1 : (short)0;
                    break;

                case Operation.Gt_Un:
                    i1 = (ushort)i1 > (ushort)i2 ? (short)1 : (short)0;
                    break;

                case Operation.Ge:
                    i1 = i1 >= i2 ? (short)1 : (short)0;
                    break;

                case Operation.Ge_Un:
                    i1 = (ushort)i1 >= (ushort)i2 ? (short)1 : (short)0;
                    break;

                case Operation.Lt:
                    i1 = i1 < i2 ? (short)1 : (short)0;
                    break;

                case Operation.Lt_Un:
                    i1 = (ushort)i1 < (ushort)i2 ? (short)1 : (short)0;
                    break;

                case Operation.Le:
                    i1 = i1 <= i2 ? (short)1 : (short)0;
                    break;

                case Operation.Le_Un:
                    i1 = (ushort)i1 <= (ushort)i2 ? (short)1 : (short)0;
                    break;

                case Operation.Add:
                    i1 = (short)(i1 + i2);
                    break;

                case Operation.Sub:
                    i1 = (short)(i1 - i2);
                    break;

                case Operation.Mul:
                    i1 = (short)(i1 * i2);
                    break;

                case Operation.Div:
                    if (i2 == 0)
                    {
                        // Division by zero, return the original tree which will throw division by zero exception
                        return tree;
                    }
                    i1 = (short)(i1 / i2);
                    break;

                default:
                    return tree;
            }

            if (tree.Operation >= Operation.Eq && tree.Operation <= Operation.Lt_Un)
            {
                return new Int32ConstantEntry(i1);
            }
            else
            {
                return new NativeIntConstantEntry(i1);
            }
        }

        private static StackEntry FoldCastOperatorWithConstantOperands(CastEntry tree)
        {
            var op1 = tree.Op1;

            Debug.Assert(op1.IsIntCnsOrI());
            int i1 = op1.GetIntConstant();

            switch (tree.Type)
            {
                case VarType.Ptr:
                    return new NativeIntConstantEntry((short)i1);
            }

            return tree;
        }

        public StackEntry FoldConstantExpression(StackEntry tree)
        {
            if (!_configuration.Optimize)
                return tree;

            if (tree is CastEntry castTree)
            {
                return FoldCastOperatorWithConstantOperands(castTree);
            }

            if (tree is BinaryOperator binaryTree)
            {
                switch (binaryTree.Op2.Type)
                {
                    case VarType.Ptr:
                        return FoldBinaryOperatorWithConstantPtrOperands(binaryTree);

                    case VarType.Int:
                        return FoldBinaryOperatorWithConstantIntOperands(binaryTree);
                }
            }

            return tree;
        }
    }
}
