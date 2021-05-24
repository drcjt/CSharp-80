﻿using System;
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

        public void Add(Instruction instruction);
        public void RemoveLast();
        public void RemoveAt(int index);
        public Instruction Last { get; }

        public Instruction this[int index] { get; }

        public int Count { get; }
    }
}
