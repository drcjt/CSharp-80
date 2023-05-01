namespace ILCompiler.Compiler.Emit
{
    public class Register8
    {
        public readonly Register Value;

        public Register8(Register value)
        {
            if (!value.IsR8()) throw new ArgumentException($"Invalid register {value}. Must be a R8 register", nameof(value));
            Value = value;
        }

        public static implicit operator Register(Register8 reg)
        {
            return reg.Value;
        }
    }
}
