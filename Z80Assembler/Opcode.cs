namespace Z80Assembler
{
    public class Opcode
    {
        private readonly string _name;

        public Opcode(string name)
        {
            _name = name;
        }

        public static readonly Opcode Ret = new("Ret");
        public static readonly Opcode Push = new("Push");
        public static readonly Opcode Pop = new("Pop");
        public static readonly Opcode Ld = new("Ld");
        public static readonly Opcode Ldir = new("Ldir");
        public static readonly Opcode Call = new("Call");
        public static readonly Opcode Rst = new("Rst");
        public static readonly Opcode Add = new("Add");
        public static readonly Opcode Adc = new("Adc");
        public static readonly Opcode Ex = new("Ex");
        public static readonly Opcode Exx = new("Exx");
        public static readonly Opcode And = new("And");
        public static readonly Opcode Or = new("Or");
        public static readonly Opcode Jp = new("Jp");
        public static readonly Opcode Sbc = new("Sbc");
        public static readonly Opcode Halt = new("Halt");
        public static readonly Opcode Inc = new("Inc");
        public static readonly Opcode Dec = new("Dec");
        public static readonly Opcode Cpl = new("Cpl");

        public static readonly Opcode Org = new("Org");
        public static readonly Opcode End = new("End");
        public static readonly Opcode Db = new("Db");
        public static readonly Opcode Defs = new("Defs");
        public static readonly Opcode Dc = new("Dc");

        public override string ToString()
        {
            return _name;
        }
    }
}
