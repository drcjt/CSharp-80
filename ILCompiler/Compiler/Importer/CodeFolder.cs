using ILCompiler.Compiler.EvaluationStack;
using System.Diagnostics;

namespace ILCompiler.Compiler.Importer
{
    internal static class CodeFolder
    {
        public static StackEntry FoldExpression(StackEntry tree)
        {
            if (tree is CastEntry castOperator && castOperator.Op1.IsIntCnsOrI())
            {
                return FoldConstantExpression(tree);
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
