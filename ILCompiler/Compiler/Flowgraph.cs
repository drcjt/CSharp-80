using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler
{
    public class Flowgraph : IFlowgraph
    {
        public void SetBlockOrder(IList<BasicBlock> blocks)
        {
            foreach (var block in blocks)
            {
                SetBlockOrder(block);
            }
        }

        public void SetBlockOrder(BasicBlock block)
        {
            foreach (var statement in block.Statements)
            {
                SetStatementSequence(statement);
            }
        }

        public void SetStatementSequence(Statement statement)
        {
            statement.TreeList = SetTreeSequence(statement.RootNode);
        }

        private static List<StackEntry> SetTreeSequence(StackEntry tree)
        {
            var orderingVisitor = new PostOrderTraversalVisitor(tree);
            orderingVisitor.WalkTree(new Edge<StackEntry>(() => tree, x => { }), null);

            var firstNode = orderingVisitor.PostOrderNodes[0];
            var lastNode = orderingVisitor.PostOrderNodes[^1];

            firstNode.Prev = null;
            lastNode.Next = null;

            return orderingVisitor.PostOrderNodes;
        }
    }

    public class PostOrderTraversalVisitor : StackEntryVisitor
    {
        public StackEntry? Current { get; private set; }
        public StackEntry? First { get; set; }

        public List<StackEntry> PostOrderNodes { get; set; } = new List<StackEntry>();

        public PostOrderTraversalVisitor(StackEntry? current)
        {
            Current = current;
        }

        public override void PostOrderVisit(Edge<StackEntry> use, StackEntry? user)
        {
            SetNext(use.Get());
        }
        private void SetNext(StackEntry entry)
        {
            PostOrderNodes.Add(entry);
            if (Current != null)
            {
                Current.Next = entry;
                entry.Prev = Current;
            }
            Current = entry;
            if (First == null)
            {
                First = Current;
            }
        }
    }
}
