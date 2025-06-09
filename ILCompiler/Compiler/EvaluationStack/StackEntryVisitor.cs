namespace ILCompiler.Compiler.EvaluationStack
{
    public class StackEntryVisitor : IStackEntryVisitor
    {
        private readonly Action<Edge<StackEntry>, StackEntry?>? _preOrderVisit;
        private readonly Action<Edge<StackEntry>, StackEntry?>? _postOrderVisit;
        public StackEntryVisitor(Action<Edge<StackEntry>, StackEntry?>? preOrderVisit = null, Action<Edge<StackEntry>, StackEntry?>? postOrderVisit = null)
        {
            _preOrderVisit = preOrderVisit;
            _postOrderVisit = postOrderVisit;
        }

        public void WalkTree(Edge<StackEntry> use, StackEntry? user)
        {
            PreOrderVisit(use, user);
            use.Get().Accept(this);
            PostOrderVisit(use, user);
        }

        public virtual void PreOrderVisit(Edge<StackEntry> use, StackEntry? user)
        {
            if (_preOrderVisit is not null)
            {
                _preOrderVisit(use, user);
            }
        }

        public virtual void PostOrderVisit(Edge<StackEntry> use, StackEntry? user)
        {
            if (_postOrderVisit is not null)
            {
                _postOrderVisit(use, user);
            }
        }

        // Leaf nodes
        public void Visit(NativeIntConstantEntry entry) { }
        public void Visit(Int32ConstantEntry entry) { }
        public void Visit(JumpEntry entry) { }
        public void Visit(LocalVariableEntry entry) { }
        public void Visit(LocalVariableAddressEntry entry) { }
        public void Visit(SymbolConstantEntry entry) { }
        public void Visit(PhiArg entry) { }
        public void Visit(CatchArgumentEntry entry) { }
        public void Visit(TokenEntry entry) { }
        public void Visit(NothingEntry entry) { }

        // Unary nodes
        public void Visit(JumpTrueEntry entry) => WalkTree(new Edge<StackEntry>(() => entry.Condition, x => entry.Condition = x), entry);
        public void Visit(NullCheckEntry entry) => WalkTree(new Edge<StackEntry>(() => entry.Op1, x => entry.Op1 = x), entry);
        public void Visit(ArrayLengthEntry entry) => WalkTree(new Edge<StackEntry>(() => entry.ArrayReference, x => entry.ArrayReference = x), entry);
        public void Visit(StoreLocalVariableEntry entry) => WalkTree(new Edge<StackEntry>(() => entry.Op1, x => entry.Op1 = x), entry);
        public void Visit(CastEntry entry) => WalkTree(new Edge<StackEntry>(() => entry.Op1, x => entry.Op1 = x), entry);
        public void Visit(UnaryOperator entry) => WalkTree(new Edge<StackEntry>(() => entry.Op1, x => entry.Op1 = x), entry);
        public void Visit(IndirectEntry entry) => WalkTree(new Edge<StackEntry>(() => entry.Op1, x => entry.Op1 = x), entry);
        public void Visit(FieldAddressEntry entry) => WalkTree(new Edge<StackEntry>(() => entry.Op1, x => entry.Op1 = x), entry);
        public void Visit(SwitchEntry entry) => WalkTree(new Edge<StackEntry>(() => entry.Op1, x => entry.Op1 = x), entry);
        public void Visit(AllocObjEntry entry) => WalkTree(new Edge<StackEntry>(() => entry.EETypeNode, x => entry.EETypeNode = x), entry);
        public void Visit(LocalHeapEntry entry) => WalkTree(new Edge<StackEntry>(() => entry.Op1, x => entry.Op1 = x), entry);
        public void Visit(PutArgTypeEntry entry) => WalkTree(new Edge<StackEntry>(() => entry.Op1, x => entry.Op1 = x), entry);
        public void Visit(ReturnEntry entry)
        {
            if (entry.Return != null)
            {
                WalkTree(new Edge<StackEntry>(() => entry.Return, x => entry.Return = x), entry);
            }
        }

        // Binary nodes
        public void Visit(BinaryOperator entry)
        {
            WalkTree(new Edge<StackEntry>(() => entry.Op2, x => entry.Op2 = x), entry);
            WalkTree(new Edge<StackEntry>(() => entry.Op1, x => entry.Op1 = x), entry);
        }
        public void Visit(StoreIndEntry entry)
        {
            WalkTree(new Edge<StackEntry>(() => entry.Op1, x => entry.Op1 = x), entry);
            WalkTree(new Edge<StackEntry>(() => entry.Addr, x => entry.Addr = x), entry);
        }
        public void Visit(IndexRefEntry entry)
        {
            WalkTree(new Edge<StackEntry>(() => entry.IndexOp, x => entry.IndexOp = x), entry);
            WalkTree(new Edge<StackEntry>(() => entry.ArrayOp, x => entry.ArrayOp = x), entry);
        }
        public void Visit(CommaEntry entry)
        {
            WalkTree(new Edge<StackEntry>(() => entry.Op1, x => entry.Op1 = x), entry);
            WalkTree(new Edge<StackEntry>(() => entry.Op2, x => entry.Op2 = x), entry);
        }
        public void Visit(BoundsCheck entry)
        {
            WalkTree(new Edge<StackEntry>(() => entry.Index, x => entry.Index = x), entry);
            WalkTree(new Edge<StackEntry>(() => entry.ArrayLength, x => entry.ArrayLength = x), entry);
        }

        // Special nodes
        public void Visit(CallEntry entry)
        {
            for (int argumentIndex = 0; argumentIndex < entry.Arguments.Count; argumentIndex++)
            {
                WalkTree(new Edge<StackEntry>(() => entry.Arguments[argumentIndex], x => entry.Arguments[argumentIndex] = x), entry);
            }
        }
        public void Visit(IntrinsicEntry entry)
        {
            for (int argumentIndex = 0; argumentIndex < entry.Arguments.Count; argumentIndex++)
            {
                WalkTree(new Edge<StackEntry>(() => entry.Arguments[argumentIndex], x => entry.Arguments[argumentIndex] = x), entry);
            }
        }
        public void Visit(PhiNode entry)
        {
            for (int argumentIndex = 0; argumentIndex < entry.Arguments.Count; argumentIndex++)
            {
                WalkTree(new Edge<StackEntry>(() => entry.Arguments[argumentIndex], x => entry.Arguments[argumentIndex] = (PhiArg)x), entry);
            }
        }

        public void Visit(ReturnExpressionEntry entry)
        {
            throw new Exception("ReturnExpressionEntry should not be visited directly. It is used during inlining of calls");
        }
    }
}
