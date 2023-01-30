namespace ILCompiler.Compiler.EvaluationStack
{
    public abstract class ConstantEntry : StackEntry
    {
        protected ConstantEntry(VarType type, int? exactSize) : base(type, exactSize)
        {
        }
    }

    public abstract class ConstantEntry<T> : ConstantEntry where T : IConvertible
    {
        public T Value { get; set; }

        protected ConstantEntry(VarType type, T value, int? exactSize) : base(type, exactSize)
        {
            Value = value;
        }
    }

    public class StringConstantEntry : ConstantEntry<String>
    {
        public string Label { get; set; } = String.Empty;
        // TODO: Should this use an exactSize of 2??
        public StringConstantEntry(string value) : base(VarType.Ptr, value, 4)
        {
        }

        public override StackEntry Duplicate()
        {
            return new StringConstantEntry(Value);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class Int32ConstantEntry : ConstantEntry<int>
    {
        public Int32ConstantEntry(int value) : base(VarType.Int, value, VarType.Int.GetTypeSize())
        {
        }

        public override StackEntry Duplicate()
        {
            return new Int32ConstantEntry(Value);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class NativeIntConstantEntry : ConstantEntry<short>
    {
        public NativeIntConstantEntry(short value, VarType type = VarType.Ptr) : base(type, value, VarType.Ptr.GetTypeSize())
        {
        }

        public override StackEntry Duplicate()
        {
            return new NativeIntConstantEntry(Value);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
