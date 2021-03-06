using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public abstract class ConstantEntry : StackEntry
    {
        protected ConstantEntry(StackValueKind kind, int? exactSize) : base(kind, exactSize)
        {
        }
    }

    public abstract class ConstantEntry<T> : ConstantEntry where T : IConvertible
    {
        public T Value { get; set; }

        protected ConstantEntry(StackValueKind kind, T value, int? exactSize) : base(kind, exactSize)
        {
            Value = value;
        }
    }

    public class StringConstantEntry : ConstantEntry<String>
    {
        public string Label { get; set; } = String.Empty;
        public StringConstantEntry(string value) : base(StackValueKind.ObjRef, value, 4)
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
        public Int32ConstantEntry(int value) : base(StackValueKind.Int32, value, 4)
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
        public NativeIntConstantEntry(short value) : base(StackValueKind.NativeInt, value, 2)
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
