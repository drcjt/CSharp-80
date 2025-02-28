namespace ILCompiler.Compiler.EvaluationStack
{
    public abstract class ConstantEntry : StackEntry
    {
        protected ConstantEntry(VarType type, int? exactSize) : base(type, exactSize)
        {
        }

        public static ConstantEntry CreateZeroConstantEntry(VarType type)
        {
            if (type.GenActualTypeIsI())
            {
                return new NativeIntConstantEntry(0);
            }
            else
            {
                return new Int32ConstantEntry(0);
            }
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

    public class SymbolConstantEntry : ConstantEntry<string>
    {
        public int Offset { get; set; } = 0;

        public SymbolConstantEntry(String name, int offset = 0) : base(VarType.Ptr, name, 2)
        {
            Offset = offset;
        }

        public override SymbolConstantEntry Duplicate()
        {
            return new SymbolConstantEntry(Value, Offset);
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
        public string SymbolName { get; private set; } = String.Empty;
        public NativeIntConstantEntry(short value, VarType type = VarType.Ptr) : base(type, value, VarType.Ptr.GetTypeSize())
        {
        }

        public NativeIntConstantEntry(string symbol, VarType type = VarType.Ptr) : base(type, 0, VarType.Ptr.GetTypeSize())
        {
            SymbolName = symbol;
        }

        public override StackEntry Duplicate()
        {
            var duplicate = new NativeIntConstantEntry(Value);
            duplicate.SymbolName = SymbolName;
            return duplicate;
        }

        public override void Accept(IStackEntryVisitor visitor) => visitor.Visit(this);
    }
}
