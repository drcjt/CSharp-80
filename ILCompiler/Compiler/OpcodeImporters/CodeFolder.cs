﻿using ILCompiler.Compiler.EvaluationStack;
using System.Diagnostics;

namespace ILCompiler.Compiler.OpcodeImporters
{
    internal static class CodeFolder
    {
        public static StackEntry FoldExpression(StackEntry tree)
        {
            if (tree is CastEntry castOperator && castOperator.Op1.IsIntCnsOrI())
            {
                return FoldConstantExpression(tree);
            }

            if (tree is BinaryOperator binaryOperator)
            {
                if (binaryOperator.Op1.IsIntCnsOrI() && binaryOperator.Op2.IsIntCnsOrI())
                {
                    return FoldBinaryOpWithBothArgumentsConstant(binaryOperator);
                }

                if (binaryOperator.Op1.IsIntCnsOrI() || binaryOperator.Op2.IsIntCnsOrI())
                {
                    return FoldBinaryOpWithOneConstantOperand(binaryOperator);
                }
            }

            return tree;
        }


        public static StackEntry FoldBinaryOpWithBothArgumentsConstant(BinaryOperator tree)
        {
            int i1;
            int i2;

            switch (tree.Op2.Type)
            {
                case VarType.Ptr:
                case VarType.Int:
                    {
                        i1 = tree.Op1.GetIntConstant();
                        i2 = tree.Op2.GetIntConstant();
                        switch (tree.Operation)
                        {
                            case Operation.Eq:
                                i1 = i1 == i2 ? 1 : 0;
                                break;

                            case Operation.Ne_Un:
                                i1 = i1 != i2 ? 1 : 0;
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
                                i1  = i1 / i2;
                                break;

                            case Operation.Lsh:
                                i1 <<= (i2 & 0x1F);
                                break;

                            case Operation.Rsh:
                                i1 >>= (i2 & 0x1F);
                                break;

                            default:
                                return tree;
                        }

                        return tree.Type == VarType.Ptr
                            ? new NativeIntConstantEntry((short)i1)
                            : new Int32ConstantEntry(i1);
                    }

                default:
                    return tree;
            }
        }

        public static StackEntry FoldBinaryOpWithOneConstantOperand(BinaryOperator tree)
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

        public static StackEntry FoldConstantExpression(StackEntry tree)
        {
            if (tree is CastEntry castOperator)
            {
                var op1 = castOperator.Op1;

                Debug.Assert(op1.IsIntCnsOrI());
                int i1 = op1.GetIntConstant();

                switch (castOperator.Type)
                {
                    case VarType.Ptr:
                        return new NativeIntConstantEntry((short)i1);

                    default:
                        return tree;
                }
            }

            if (tree is BinaryOperator binaryOperator)
            {
                if (binaryOperator.Type == VarType.Ptr)
                {
                    var i1 = (NativeIntConstantEntry)binaryOperator.Op1;
                    var i2 = (NativeIntConstantEntry)binaryOperator.Op2;

                    switch (binaryOperator.Operation)
                    {
                        case Operation.Add:
                            return new NativeIntConstantEntry((short)(i1.Value + i2.Value));
                        case Operation.Sub:
                            return new NativeIntConstantEntry((short)(i1.Value - i2.Value));
                        case Operation.Mul:
                            return new NativeIntConstantEntry((short)(i1.Value * i2.Value));
                        case Operation.Div:
                            return new NativeIntConstantEntry((short)(i1.Value / i2.Value));

                        default:
                            throw new NotImplementedException($"Cannot fold constant expression with operation {binaryOperator.Operation}");
                    }
                }

                if (binaryOperator.Type.IsInt())
                {
                    var i1 = (Int32ConstantEntry)binaryOperator.Op1;
                    var i2 = (Int32ConstantEntry)binaryOperator.Op2;

                    switch (binaryOperator.Operation)
                    {
                        case Operation.Add:
                            return new Int32ConstantEntry(i1.Value + i2.Value);
                        case Operation.Sub:
                            return new Int32ConstantEntry(i1.Value - i2.Value);
                        case Operation.Mul:
                            return new Int32ConstantEntry(i1.Value * i2.Value);
                        case Operation.Div:
                            return new Int32ConstantEntry(i1.Value / i2.Value);

                        default:
                            throw new NotImplementedException($"Cannot fold constant expression with operation {binaryOperator.Operation}");
                    }
                }
            }

            return tree;
        }
    }
}
