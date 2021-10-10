using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class LocalVariableAddressEntry : StackEntry
    {
        public int LocalNumber { get; }

        public LocalVariableAddressEntry(int localNumber) : base(StackValueKind.ObjRef, 4)
        {
            Operation = Operation.LocalVariableAddress;
            LocalNumber = localNumber;
        }

        public override StackEntry Duplicate()
        {
            return new LocalVariableAddressEntry(LocalNumber);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
