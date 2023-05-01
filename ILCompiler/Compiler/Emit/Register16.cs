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

        public static implicit operator Register(Register16 reg)
        {
            return reg.Value;
        }

        public static MemoryOperand operator +(Register16 left, short displacement)
        {
            return new MemoryOperand(left, displacement);
        }
    }
}
