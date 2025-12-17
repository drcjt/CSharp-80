namespace ILCompiler.Compiler.ValueNumbering
{
    public enum VNFunc
    {
        Neg,
        Not,

        Add,
        Sub,
        Mul,
        Div,
        Div_Un,
        Rem,
        Rem_Un,
        And,
        Or,
        Xor,

        Eq,
        Ge,
        Gt,
        Le,
        Lt,
        Ne_Un,
        Ge_Un,
        Gt_Un,
        Le_Un,
        Lt_Un,

        Lsh,
        Rsh,

        InitVal,

        PtrToLoc,

        MemOpaque,

        Cast,
    }
}
