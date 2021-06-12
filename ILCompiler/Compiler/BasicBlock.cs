using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Interfaces;
using ILCompiler.z80;
using System;
using System.Collections.Generic;
using Instruction = ILCompiler.z80.Instruction;

namespace ILCompiler.Compiler
{
    public class BasicBlock
    {
        public BasicBlock Next { get; set; }
        public int StartOffset { get; set; }

        // TODO: This should really be a proper intermediate representation rather
        // the z80 instructions directly
        public IList<Instruction> Instructions { get; set; } = new List<Instruction>();

        public bool Marked { get; set; } = false;

        public string Label => $"bb{_id}";
        private readonly int _id;
        private static int nextId = 0;

        public BasicBlock(int offset)
        {
            StartOffset = offset;
            _id = nextId++;
        }

        public void Append(Instruction instruction)
        {
            Instructions.Add(instruction);
        }
    }
}
