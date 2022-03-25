using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class LocalVariableAddressEntry : StackEntry, ILocalVariable
    {
        public int LocalNumber { get; }

        public LocalVariableAddressEntry(int localNumber) : base(StackValueKind.NativeInt, 2)
        {
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
