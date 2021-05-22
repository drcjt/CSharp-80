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

        public Opcode(string name)
        {
            _name = name;
        }

        public static readonly Opcode Ret = new("Ret");
        public static readonly Opcode Push = new("Push");
        public static readonly Opcode Pop = new("Pop");
        public static readonly Opcode Ld = new("Ld");
        public static readonly Opcode Call = new("Call");
        public static readonly Opcode Add = new("Add");

        public override string ToString()
        {
            return _name;
        }
    }
}
