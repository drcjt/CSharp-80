using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler
{
    public class Flowgraph : IFlowgraph
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
                StackEntry? current = null;
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
        public StackEntry? Current { get; private set; }
        public StackEntry? First { get; set; }

        public PostOrderTraversalVisitor(StackEntry? current)
        {
            Current = current;
        }

        public void Visit(NativeIntConstantEntry entry)
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
            entry.Op1.Accept(this);
            entry.Addr.Accept(this);
            SetNext(entry);
        }

        public void Visit(IndirectEntry entry)
        {
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
            entry.Op2.Accept(this);
            entry.Op1.Accept(this);
            SetNext(entry);
        }

        public void Visit(UnaryOperator entry)
        {
            entry.Op1.Accept(this);
            SetNext(entry);
        }

        public void Visit(LocalVariableEntry entry)
        {
            SetNext(entry);
        }

        public void Visit(LocalVariableAddressEntry entry)
        {
            SetNext(entry);
        }

        public void Visit(StoreLocalVariableEntry entry)
        {
            entry.Op1.Accept(this);
            SetNext(entry);
        }

        public void Visit(FieldAddressEntry entry)
        {
            entry.Op1.Accept(this);
            SetNext(entry);
        }

        public void Visit(StaticFieldEntry entry)
        {
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

        public void Visit(CastEntry entry)
        {
            entry.Op1.Accept(this);
            SetNext(entry);
        }

        public void Visit(SwitchEntry entry)
        {
            entry.Op1.Accept(this);
            SetNext(entry);
        }

        public void Visit(AllocObjEntry entry)
        {
            SetNext(entry);
        }

        public void Visit(LocalHeapEntry entry)
        {
            entry.Op1.Accept(this);
            SetNext(entry);
        }

        public void Visit(IndexRefEntry entry)
        {
            entry.IndexOp.Accept(this);
            entry.ArrayOp.Accept(this);
            SetNext(entry);
        }

        public void Visit(PutArgTypeEntry entry)
        {
            entry.Op1.Accept(this);
            SetNext(entry);
        }

        public void Visit(CommaEntry entry)
        {
            entry.Op1.Accept(this);
            entry.Op2.Accept(this);
            SetNext(entry);
        }

        public void Visit(PhiNode entry)
        {
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
