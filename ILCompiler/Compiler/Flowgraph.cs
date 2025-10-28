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

        public void InsertStatementAfter(BasicBlock block, Statement insertionPoint, Statement statement)
        {
            var index = block.Statements.IndexOf(insertionPoint);
            if (index == -1)
            {
                throw new ArgumentException("Insertion point not found in block.");
            }
            if (index == block.Statements.Count - 1)
            {
                // Insertion point is the last statement
                InsertStatementAtEnd(block, statement);
            }
            else
            {
                // Insert the statement after the insertion point
                block.Statements.Insert(index + 1, statement);
                // Update the evaluation order
                SetStatementSequence(statement);
            }
        }

        public void InsertStatementNearEnd(BasicBlock block, Statement statement)
        {
            if (block.HasTerminator)
            {
                throw new NotImplementedException("Inserting before terminator not implemented.");
            }
            else
            {
                InsertStatementAtEnd(block, statement);
            }              
        }

        public void InsertStatementAtEnd(BasicBlock block, Statement statement)
        {
            // Insert the statement at the end of the block
            block.Statements.Add(statement);
            // Update the evaluation order
            SetStatementSequence(statement);
        }

        public void RemoveStatement(BasicBlock block, Statement statement)
        {
            block.Statements.Remove(statement);
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

        public override WalkResult PostOrderVisit(Edge<StackEntry> use, StackEntry? user)
        {
            SetNext(use.Get());
            return WalkResult.Continue;
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
