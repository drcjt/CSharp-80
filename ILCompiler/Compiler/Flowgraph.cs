using ILCompiler.Compiler.EvaluationStack;
using System.Collections.Generic;

namespace ILCompiler.Compiler
{
    public class Flowgraph
    {
        public void SetBlockOrder(IList<BasicBlock> blocks)
        {
            // Put code here to set execution order within the basic blocks
            foreach (var block in blocks)
            {
                foreach (var statement in block.Statements)
                {
                    PostOrderTraversal(statement);
                }
            }

            // canonical post order traversal of the HIR tree

            // Will iterate the StackEntrys in each basic block and traverse the statement tree
            // setting the Next property on each stack entry to indicate the true execution order within the block
        }

        private void PostOrderTraversal(StackEntry entry)
        {
            var orderingVisitor = new PostOrderTraversalVisitor();
            entry.Accept(orderingVisitor);
        }
    }

    public class PostOrderTraversalVisitor : IStackEntryVisitor
    {
        private StackEntry _current;
        public void Visit(ConstantEntry entry)
        {
            //entry.Accept(this);
            SetNext(entry);
        }

        public void Visit(StoreIndEntry entry)
        {
            entry.Op1.Accept(this);
            entry.Addr.Accept(this);
            SetNext(entry);
        }

        public void Visit(JumpTrueEntry entry)
        {
            entry.Condition.Accept(this);
            SetNext(entry);
        }

        public void Visit(ReturnEntry entry)
        {
            if (entry.Return != null)
            {
                entry.Return.Accept(this);
            }
            SetNext(entry);
        }

        public void Visit(BinaryOperator entry)
        {
            entry.Op1.Accept(this);
            entry.Op2.Accept(this);
            SetNext(entry);
        }

        public void Visit(LocalVariableEntry entry)
        {
            SetNext(entry);
        }

        public void Visit(StoreLocalVariableEntry entry)
        {
            entry.Op1.Accept(this);
            SetNext(entry);
        }

        public void Visit(CallEntry entry)
        {
            foreach (var argument in entry.Arguments)
            {
                argument.Accept(this);
            }
            SetNext(entry);
        }

        public void Visit(IntrinsicEntry entry)
        {
            foreach (var argument in entry.Arguments)
            {
                argument.Accept(this);
            }
            SetNext(entry);
        }

        private void SetNext(StackEntry entry)
        {
            if (_current != null)
            {
                _current.Next = entry;
            }
            _current = entry;
        }
    }
}
