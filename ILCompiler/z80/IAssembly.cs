using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILCompiler.z80
{
    public interface IAssembly
    {
        public void Label(string label);
        public void Write(string filePath, string inputFilePath);

        public void Add(Instruction instruction);
        public void RemoveLast();
        public Instruction Last { get; }
    }
}
