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

        public void Ret();
        public void Pop(R16Type target);
        public void Push(R16Type target);
        public void Ld(R16Type target, sbyte source);
        public void Ld(R8Type target, R8Type source);
        public void Call(string label);
        public void Call(UInt16 target);
        public void Add(R16Type target, R16Type source);
    }
}
