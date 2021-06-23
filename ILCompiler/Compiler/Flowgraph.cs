﻿using ILCompiler.Compiler.EvaluationStack;
using System.Collections.Generic;

namespace ILCompiler.Compiler
{
    public class Flowgraph
    {
        /// <summary>
        /// Performs a canonical post order traversal of the HIR tree
        /// Iterates through all statements in each block setting the Next property on each StackEntry
        /// to indicate the true execution order within the block
        /// </summary>
        /// <param name="blocks"></param>
        public void SetBlockOrder(IList<BasicBlock> blocks)
        {
            foreach (var block in blocks)
            {
                StackEntry current = null;
                foreach (var statement in block.Statements)
                {
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
        public StackEntry Current { get; private set; }
        public StackEntry First { get; set; }

        public PostOrderTraversalVisitor(StackEntry current)
        {
            Current = current;
        }

        public void Visit(Int16ConstantEntry entry)
        {
            SetNext(entry);
        }

        public void Visit(Int32ConstantEntry entry)
        {
            SetNext(entry);
        }

        public void Visit(StringConstantEntry entry)
        {
            SetNext(entry);
        }

        public void Visit(StoreIndEntry entry)
        {
            entry.Addr.Accept(this);
            entry.Op1.Accept(this);
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