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
            tree.Accept(orderingVisitor);

            var firstNode = orderingVisitor.PostOrderNodes[0];
            var lastNode = orderingVisitor.PostOrderNodes[^1];

            firstNode.Prev = null;
            lastNode.Next = null;

            return orderingVisitor.PostOrderNodes;
        }
    }

    public class PostOrderTraversalVisitor : IStackEntryVisitor
    {
        public StackEntry? Current { get; private set; }
        public StackEntry? First { get; set; }

        public List<StackEntry> PostOrderNodes { get; set; } = new List<StackEntry>();

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

        public void Visit(SymbolConstantEntry entry)
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

        public void Visit(NullCheckEntry entry)
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
            entry.EETypeNode.Accept(this);
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

        public void Visit(BoundsCheck entry)
        {
            entry.Index.Accept(this);
            entry.ArrayLength.Accept(this);
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

        public void Visit(PhiArg entry)
        {
            SetNext(entry);
        }

        public void Visit(CatchArgumentEntry entry)
        {
            SetNext(entry);
        }

        public void Visit(TokenEntry entry)
        {
            SetNext(entry);
        }

        public void Visit(ArrayLengthEntry entry)
        {
            entry.ArrayReference.Accept(this);
            SetNext(entry);
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
