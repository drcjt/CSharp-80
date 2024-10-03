using ILCompiler.Compiler.EvaluationStack;
using System.Diagnostics;

namespace ILCompiler.Compiler.LinearIR
{
    public static class LIR
    {
        public static Range SeqTree(StackEntry tree)
        {
            var orderingVisitor = new PostOrderTraversalVisitor(tree);
            tree.Accept(orderingVisitor);

            var lastNode = tree;
            var firstNode = orderingVisitor.First;

            Debug.Assert(firstNode != null);

            lastNode.Next = null;
            firstNode.Prev = null;

            return new Range(firstNode, lastNode);
        }
    }
}
