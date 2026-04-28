using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler
{
    public class FlowGraph : IFlowgraph
    {
        public IList<BasicBlock> Blocks { get; } = [];

        public IList<EHClause> EhClauses { get; } = [];

        public IEnumerable<BasicBlock> VisitAllSuccs(BasicBlock b)
        {
            foreach (BasicBlock succ in b.Successors)
                yield return succ;

            foreach (BasicBlock ehSucc in GetEhSuccs(b))
                yield return ehSucc;
        }

        private IEnumerable<BasicBlock> GetEhSuccs(BasicBlock b)
        {
            foreach (EHClause clause in EhClauses)
            {
                if (IsInTryRegion(b, clause))
                {
                    yield return clause.HandlerBegin;
                }
            }
        }

        public bool IsInHandlerRegion(BasicBlock b, EHClause c)
        {
            bool inRegion = false;
            foreach (BasicBlock block in Blocks)
            {
                if (block == c.HandlerBegin)
                    inRegion = true;
                if (block == c.HandlerEnd)
                    break;
                if (block == b && inRegion)
                    return true;
            }
            return false;
        }

        bool IsInTryRegion(BasicBlock b, EHClause c)
        {
            bool inRegion = false;

            foreach (BasicBlock block in Blocks)
            {
                if (block == c.TryBegin)
                    inRegion = true;

                if (block == b && inRegion)
                    return true;

                if (block == c.TryLast)
                    break;
            }

            return false;
        }

        public void SetBlockOrder()
        {
            foreach (var block in Blocks)
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
}
