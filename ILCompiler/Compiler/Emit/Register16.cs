namespace ILCompiler.Compiler.Emit
{
    public class Register16
    {
        public readonly Register Value;

        public Register16(Register value)
        {
            if (!value.IsR16()) throw new ArgumentException($"Invalid register {value}. Must be a R16 register", nameof(value));
            Value = value;
        }

        public static implicit operator Register(Register16 reg) => reg.Value;

        public bool IsIndexRegister() => Value.IsIndexRegister();

        public static MemoryOperand operator +(Register16 left, short displacement) => new MemoryOperand(left, displacement);
    }
}
