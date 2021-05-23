using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILCompiler.z80
{
    public interface IZ80Assembly
    {
        public void Label(string label);
        public void Write(string filePath, string inputFilePath);

        public void Add(Z80Instruction instruction);
        public void RemoveLast();
        public Z80Instruction Last { get; }
    }
}
