namespace ILCompiler.Compiler.Emit
{
    public enum Opcode
    {
        None,

        Org,
        End,
        Equ,

        Db,
        Defs,
        Dc,
        Dw,

        Adc,
        Add,
        And,
        Call,
        Cpl,
        Dec,
        Ex,
        Exx,
        Halt,
        Inc,
        Jp,
        Jr,
        Ld,
        Ldir,
        Or,
        Ret,
        Rr,
        Rst,
        Push,
        Pop,
        Sbc,
        Srl,       
    }
}
