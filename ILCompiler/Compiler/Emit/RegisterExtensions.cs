namespace ILCompiler.Compiler.Emit
{
    public static class RegisterExtensions
    {
        public static bool IsR8(this Register register) => Register.A <= register && register <= Register.L;

        public static bool IsR16(this Register register) => Register.AF <= register && register <= Register.PC;

        public static bool IsIndexRegister(this Register register) => register == Register.IX || register == Register.IY;
    }
}
