namespace ILCompiler.Compiler.EvaluationStack
{
    public class GenericStackEntryAdapter : IStackEntryVisitor
    {
        IGenericStackEntryVisitor _genericStackEntryVisitor;
        public GenericStackEntryAdapter(IGenericStackEntryVisitor genericVisitor)
        {
            _genericStackEntryVisitor = genericVisitor;
        }

        public void Visit(Int32ConstantEntry entry)
        {
            _genericStackEntryVisitor.Visit<Int32ConstantEntry>(entry);
        }

        public void Visit(StringConstantEntry entry)
        {
            _genericStackEntryVisitor.Visit<StringConstantEntry>(entry);
        }

        public void Visit(StoreIndEntry entry)
        {
            _genericStackEntryVisitor.Visit<StoreIndEntry>(entry);
        }

        public void Visit(JumpTrueEntry entry)
        {
            _genericStackEntryVisitor.Visit<JumpTrueEntry>(entry);
        }

        public void Visit(JumpEntry entry)
        {
            _genericStackEntryVisitor.Visit<JumpEntry>(entry);
        }

        public void Visit(ReturnEntry entry)
        {
            _genericStackEntryVisitor.Visit<ReturnEntry>(entry);
        }

        public void Visit(BinaryOperator entry)
        {
            _genericStackEntryVisitor.Visit<BinaryOperator>(entry);
        }

        public void Visit(LocalVariableEntry entry)
        {
            _genericStackEntryVisitor.Visit<LocalVariableEntry>(entry);
        }

        public void Visit(LocalVariableAddressEntry entry)
        {
            _genericStackEntryVisitor.Visit<LocalVariableAddressEntry>(entry);
        }

        public void Visit(StoreLocalVariableEntry entry)
        {
            _genericStackEntryVisitor.Visit<StoreLocalVariableEntry>(entry);
        }

        public void Visit(CallEntry entry)
        {
            _genericStackEntryVisitor.Visit<CallEntry>(entry);
        }

        public void Visit(IntrinsicEntry entry)
        {
            _genericStackEntryVisitor.Visit<IntrinsicEntry>(entry);
        }

        public void Visit(CastEntry entry)
        {
            _genericStackEntryVisitor.Visit<CastEntry>(entry);
        }

        public void Visit(UnaryOperator entry)
        {
            _genericStackEntryVisitor.Visit<UnaryOperator>(entry);
        }

        public void Visit(IndirectEntry entry)
        {
            _genericStackEntryVisitor.Visit<IndirectEntry>(entry);
        }

        public void Visit(FieldAddressEntry entry)
        {
            _genericStackEntryVisitor.Visit<FieldAddressEntry>(entry);
        }

        public void Visit(SwitchEntry entry)
        {
            _genericStackEntryVisitor.Visit<SwitchEntry>(entry);
        }
    }
}
