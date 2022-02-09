using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class LocalVariableEntry : StackEntry, ILocalVariable
    {
        public int LocalNumber { get; }
        public LocalVariableEntry(int localNumber, StackValueKind kind, int? exactSize) : base(kind, exactSize)
        {
            LocalNumber = localNumber;
        }

        public override StackEntry Duplicate()
        {
            return new LocalVariableEntry(LocalNumber, Kind, ExactSize);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
