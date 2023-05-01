namespace ILCompiler.Compiler.Emit
{
    public static class Registers
    {
        public static readonly MemoryOperandFactory __ = new MemoryOperandFactory();

        public static readonly Register8 A = new(Register.A);
        public static readonly Register8 F = new(Register.F);
        public static readonly Register8 B = new(Register.B);
        public static readonly Register8 C = new(Register.C);
        public static readonly Register8 D = new(Register.D);
        public static readonly Register8 E = new(Register.E);
        public static readonly Register8 H = new(Register.H);
        public static readonly Register8 L = new(Register.L);

        public static readonly Register16 AF = new(Register.AF);
        public static readonly Register16 BC = new(Register.BC);
        public static readonly Register16 DE = new(Register.DE);
        public static readonly Register16 HL = new(Register.HL);

        public static readonly Register16 SP = new(Register.SP);
        public static readonly Register16 PC = new(Register.PC);

        public static readonly Register16 IX = new(Register.IX);
        public static readonly Register16 IY = new(Register.IY);
    }
}
