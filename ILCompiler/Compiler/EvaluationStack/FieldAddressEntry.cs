namespace ILCompiler.Compiler.EvaluationStack
{
    public class FieldAddressEntry : StackEntry
    {
        public uint Offset { get; }

        public StackEntry Op1 { get; }

        public string Name { get; }

        public FieldAddressEntry(String name, StackEntry op1, uint offset) : base(VarType.Ptr, 2)
        {
            Name = name;
            Op1 = op1;
            Offset = offset;
        }

        public override FieldAddressEntry Duplicate()
        {
            return new FieldAddressEntry(Name, Op1.Duplicate(), Offset);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
