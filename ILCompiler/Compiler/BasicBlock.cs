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

        public string Label { get; private set; }

        public BasicBlock(int offset)
        {
            StartOffset = offset;
            Label = LabelGenerator.GetLabel(LabelType.BasicBlock);
        }

        public void Append(Instruction instruction)
        {
            Instructions.Add(instruction);
        }
    }
}
