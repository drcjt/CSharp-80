using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILCompiler.z80
{
    public class Opcode
    {
        private readonly string _name;
        private readonly bool _pseudo;

        public Opcode(string name, bool pseudo = false)
        {
            _name = name;
            _pseudo = pseudo;
        }

        public static readonly Opcode Ret = new("Ret");
        public static readonly Opcode Push = new("Push");
        public static readonly Opcode Pop = new("Pop");
        public static readonly Opcode Ld = new("Ld");
        public static readonly Opcode Call = new("Call");
        public static readonly Opcode Add = new("Add");
        public static readonly Opcode Ex = new("Ex");
        public static readonly Opcode Or = new("Or");
        public static readonly Opcode Jp = new("Jp");
        public static readonly Opcode Sbc = new("Sbc");

        public static readonly Opcode Org = new("Org", true);
        public static readonly Opcode End = new("End", true);
        public override string ToString()
        {
            return _name;
        }
    }
}
