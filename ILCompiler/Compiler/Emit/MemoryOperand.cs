namespace ILCompiler.Compiler.Emit
{
    public class MemoryOperand
    {
        public readonly Register Register;

        public readonly short Displacement;

        public readonly string? Label;

        internal MemoryOperand(Register register, short displacement, string? label = null)
        {
            Register = register;
            Displacement = displacement;
            Label = label;
        }
    }
}
