namespace ILCompiler.Compiler.EvaluationStack
{
    public class LocalVariableAddressEntry : LocalVariableCommon
    {
        public LocalVariableAddressEntry(int localNumber) : base(VarType.Ptr, VarType.Ptr.GetTypeSize())
        {
            LocalNumber = localNumber;
        }

        public override StackEntry Duplicate()
        {
            return new LocalVariableAddressEntry(LocalNumber);
        }

        public override void Accept(IStackEntryVisitor visitor) => visitor.Visit(this);
    }
}
