using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z80Assembler
{
    public class Label
    {
        public readonly string Name;

        public readonly ulong Id;

        public override string ToString() => $"{Name}@{Id}";

        public Label(string? name, ulong id)
        {
            Name = name ?? "__label";
            Id = id;
        }
    }
}
