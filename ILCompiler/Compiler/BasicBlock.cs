using ILCompiler.Compiler.EvaluationStack;
using System.Collections.Generic;
using Instruction = ILCompiler.z80.Instruction;

namespace ILCompiler.Compiler
{
    public class BasicBlock
    {
        public BasicBlock Next { get; set; }
        public int StartOffset { get; set; }

        // High level intermediate representation - main output of importation process
        public IList<StackEntry> Statements { get; } = new List<StackEntry>();

        public StackEntry FirstNode { get; set; }
        public bool Marked { get; set; } = false;

        public string Label { get; private set; }

        public BasicBlock(int offset)
        {
            StartOffset = offset;
            Label = LabelGenerator.GetLabel(LabelType.BasicBlock);
        }
    }
}
