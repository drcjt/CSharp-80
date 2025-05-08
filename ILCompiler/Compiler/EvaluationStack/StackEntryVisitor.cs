namespace ILCompiler.Compiler.EvaluationStack
{
    public class StackEntryVisitor : IStackEntryVisitor
    {
        private readonly Action<StackEntry>? _preOrderVisit;
        private readonly Action<StackEntry>? _postOrderVisit;
        public StackEntryVisitor(Action<StackEntry>? preOrderVisit = null, Action<StackEntry>? postOrderVisit = null)
        {
            _preOrderVisit = preOrderVisit;
            _postOrderVisit = postOrderVisit;
        }

        public void WalkTree(StackEntry entry)
        {
            entry.Accept(this);
        }

        public virtual void PreOrderVisit(StackEntry entry)
        {
            if (_preOrderVisit is not null)
            {
                _preOrderVisit(entry);
            }
        }

        public virtual void PostOrderVisit(StackEntry entry)
        {
            if (_postOrderVisit is not null)
            {
                _postOrderVisit(entry);
            }
        }

        public void Visit(NativeIntConstantEntry entry)
        {
            PreOrderVisit(entry);
            PostOrderVisit(entry);
        }

        public void Visit(Int32ConstantEntry entry)
        {
            PreOrderVisit(entry);
            PostOrderVisit(entry);
        }

        public void Visit(StoreIndEntry entry)
        {
            PreOrderVisit(entry);
            entry.Op1.Accept(this);
            entry.Addr.Accept(this);
            PostOrderVisit(entry);
        }

        public void Visit(JumpTrueEntry entry)
        {
            PreOrderVisit(entry);
            entry.Condition.Accept(this);
            PostOrderVisit(entry);
        }

        public void Visit(JumpEntry entry)
        {
            PreOrderVisit(entry);
            PostOrderVisit(entry);
        }

        public void Visit(ReturnEntry entry)
        {
            PreOrderVisit(entry);
            if (entry.Return != null)
            {
                entry.Return.Accept(this);
            }
            PostOrderVisit(entry);
        }

        public void Visit(BinaryOperator entry)
        {
            PreOrderVisit(entry);
            entry.Op2.Accept(this);
            entry.Op1.Accept(this);
            PostOrderVisit(entry);
        }

        public void Visit(LocalVariableEntry entry)
        {
            PreOrderVisit(entry);
            PostOrderVisit(entry);
        }

        public void Visit(LocalVariableAddressEntry entry)
        {
            PreOrderVisit(entry);
            PostOrderVisit(entry);
        }

        public void Visit(StoreLocalVariableEntry entry)
        {
            PreOrderVisit(entry);
            entry.Op1.Accept(this);
            PostOrderVisit(entry);
        }

        public void Visit(CallEntry entry)
        {
            PreOrderVisit(entry);
            foreach (var argument in entry.Arguments)
            {
                argument.Accept(this);
            }
            PostOrderVisit(entry);
        }

        public void Visit(IntrinsicEntry entry)
        {
            PreOrderVisit(entry);
            foreach (var argument in entry.Arguments)
            {
                argument.Accept(this);
            }
            PostOrderVisit(entry);
        }

        public void Visit(CastEntry entry)
        {
            PreOrderVisit(entry);
            entry.Op1.Accept(this);
            PostOrderVisit(entry);
        }

        public void Visit(UnaryOperator entry)
        {
            PreOrderVisit(entry);
            entry.Op1.Accept(this);
            PostOrderVisit(entry);
        }

        public void Visit(IndirectEntry entry)
        {
            PreOrderVisit(entry);
            entry.Op1.Accept(this);
            PostOrderVisit(entry);
        }

        public void Visit(FieldAddressEntry entry)
        {
            PreOrderVisit(entry);
            entry.Op1.Accept(this);
            PostOrderVisit(entry);
        }

        public void Visit(SwitchEntry entry)
        {
            PreOrderVisit(entry);
            entry.Op1.Accept(this);
            PostOrderVisit(entry);
        }

        public void Visit(AllocObjEntry entry)
        {
            PreOrderVisit(entry);
            entry.EETypeNode.Accept(this);
            PostOrderVisit(entry);
        }

        public void Visit(LocalHeapEntry entry)
        {
            PreOrderVisit(entry);
            entry.Op1.Accept(this);
            PostOrderVisit(entry);
        }

        public void Visit(IndexRefEntry entry)
        {
            PreOrderVisit(entry);
            entry.IndexOp.Accept(this);
            entry.ArrayOp.Accept(this);
            PostOrderVisit(entry);
        }

        public void Visit(PutArgTypeEntry entry)
        {
            PreOrderVisit(entry);
            entry.Op1.Accept(this);
            PostOrderVisit(entry);
        }

        public void Visit(SymbolConstantEntry entry)
        {
            PreOrderVisit(entry);
            PostOrderVisit(entry);
        }

        public void Visit(CommaEntry entry)
        {
            PreOrderVisit(entry);
            entry.Op1.Accept(this);
            entry.Op2.Accept(this);
            PostOrderVisit(entry);
        }

        public void Visit(PhiNode entry)
        {
            PreOrderVisit(entry);
            PostOrderVisit(entry);
        }

        public void Visit(PhiArg entry)
        {
            PreOrderVisit(entry);
            PostOrderVisit(entry);
        }

        public void Visit(BoundsCheck entry)
        {
            PreOrderVisit(entry);
            entry.Index.Accept(this);
            entry.ArrayLength.Accept(this);
            PostOrderVisit(entry);
        }

        public void Visit(NullCheckEntry entry)
        {
            PreOrderVisit(entry);
            entry.Op1.Accept(this);
            PostOrderVisit(entry);
        }

        public void Visit(CatchArgumentEntry entry)
        {
            PreOrderVisit(entry);
            PostOrderVisit(entry);
        }

        public void Visit(TokenEntry entry)
        {
            PreOrderVisit(entry);
            PostOrderVisit(entry);
        }

        public void Visit(ArrayLengthEntry entry)
        {
            PreOrderVisit(entry);
            entry.ArrayReference.Accept(this);
            PostOrderVisit(entry);
        }
    }
}
