namespace ILCompiler.Compiler.EvaluationStack
{
    public class StoreLocalVariableEntry : StackEntry, ILocalVariable
    {
        public StackEntry Op1 { get; }

        public int LocalNumber { get; }

        public bool IsParameter { get; }

        public StoreLocalVariableEntry(int localNumber, bool parameter, StackEntry op1) : base(VarType.Void)
        {
            LocalNumber = localNumber;
            IsParameter = parameter;
            Op1 = op1;
        }

        public override StackEntry Duplicate()
        {
            return new StoreLocalVariableEntry(LocalNumber, IsParameter, Op1.Duplicate());
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
