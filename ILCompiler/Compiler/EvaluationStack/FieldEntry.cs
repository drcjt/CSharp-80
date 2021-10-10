using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class FieldEntry : StackEntry
    {
        public uint? Offset;
        public string Name;

        public StackEntry Op1 { get; }

        public FieldEntry(StackEntry op1, string name, uint? offset, int size, StackValueKind kind) : base(kind, size)
        {
            Operation = Operation.Field;
            Op1 = op1;
            Name = name;
            Offset = offset;
        }

        public override FieldEntry Duplicate()
        {
            return new FieldEntry(Op1.Duplicate(), Name, Offset, ExactSize.Value, Kind);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
