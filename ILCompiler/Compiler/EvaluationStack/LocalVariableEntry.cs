using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class LocalVariableEntry : StackEntry
    {
        public int LocalNumber { get; }
        public LocalVariableEntry(int localNumber, StackValueKind kind) : base(kind)
        {
            Operation = Operation.LocalVariable;
            LocalNumber = localNumber;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
