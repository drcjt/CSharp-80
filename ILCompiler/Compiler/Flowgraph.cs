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
                StackEntry current = null;
                foreach (var statement in block.Statements)
                {
                    // canonical post order traversal of the HIR tree

                    // Will iterate the StackEntrys in each basic block and traverse the statement tree
                    // setting the Next property on each stack entry to indicate the true execution order within the block
                    var orderingVisitor = new PostOrderTraversalVisitor(current);
                    statement.Accept(orderingVisitor);
                    var first = orderingVisitor.First;
                    current = orderingVisitor.Current;

                    if (block.FirstNode == null)
                    {
                        block.FirstNode = first;
                    }
                }
            }
        }
    }

    public class PostOrderTraversalVisitor : IStackEntryVisitor
    {
        public PostOrderTraversalVisitor(StackEntry current)
        {
            Current = current;
        }

        public StackEntry Current { get; private set; }
        public StackEntry First { get; set; }
        public void Visit(ConstantEntry entry)
        {
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

        public void Visit(JumpEntry entry)
        {
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
            if (Current != null)
            {
                Current.Next = entry;
            }
            Current = entry;
            if (First == null)
            {
                First = Current;
            }
        }
    }
}
