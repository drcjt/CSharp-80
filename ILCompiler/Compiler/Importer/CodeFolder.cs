using ILCompiler.Compiler.EvaluationStack;
using System.Diagnostics;

namespace ILCompiler.Compiler.Importer
{
    internal static class CodeFolder
    {
        public static StackEntry FoldExpression(StackEntry tree)
        {
            if (tree is CastEntry castOperator)
            {
                if (castOperator.Op1.IsIntCnsOrI())
                {
                    return FoldConstantExpression(tree);
                }
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

            return tree;
        }
    }
}
