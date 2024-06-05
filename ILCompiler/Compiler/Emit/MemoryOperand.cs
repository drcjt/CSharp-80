using System.Diagnostics;

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

            //Debug.Assert((register == Register.IX || register == Register.IY) && displacement > -129, "Index register offset cannot be less than -128");
            //Debug.Assert((register == Register.IX || register == Register.IY) && displacement < 128, "Index register offset cannot be more than 127");
        }
    }
}
