namespace ILCompiler.Compiler.EvaluationStack
{
    public class LocalVariableEntry : StackEntry, ILocalVariable
    {
        public int LocalNumber { get; }
        public int SsaNumber { get; }
        public LocalVariableEntry(int localNumber, VarType type, int? exactSize) : base(type, exactSize)
        {
            LocalNumber = localNumber;
        }

        public override StackEntry Duplicate()
        {
            return new LocalVariableEntry(LocalNumber, Type, ExactSize);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
