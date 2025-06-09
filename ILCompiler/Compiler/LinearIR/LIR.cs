using ILCompiler.Compiler.EvaluationStack;
using System.Diagnostics;

namespace ILCompiler.Compiler.LinearIR
{
    public static class LIR
    {
        public static Range SeqTree(StackEntry tree)
        {
            var orderingVisitor = new PostOrderTraversalVisitor(tree);
            orderingVisitor.WalkTree(new Edge<StackEntry>(() => tree, x => { }), null);

            var lastNode = tree;
            var firstNode = orderingVisitor.First;

            Debug.Assert(firstNode != null);

            lastNode.Next = null;
            firstNode.Prev = null;

            return new Range(firstNode, lastNode);
        }
    }
}
