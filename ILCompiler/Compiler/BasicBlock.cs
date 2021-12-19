using ILCompiler.Compiler.EvaluationStack;
using System.Collections.Generic;

namespace ILCompiler.Compiler
{
    public class BasicBlock
    {
        public BasicBlock? Next { get; set; }
        public int StartOffset { get; set; }

        public uint PostOrderNum { get; set; }

        public BasicBlock? ImmediateDominator { get; set; }
        public IList<BasicBlock> Successors = new List<BasicBlock>();
        public IList<BasicBlock> Predecessors = new List<BasicBlock>();

        // High level intermediate representation - main output of importation process
        public IList<StackEntry> Statements { get; } = new List<StackEntry>();

        public StackEntry? FirstNode { get; set; }
        public bool Marked { get; set; } = false;

        public string Label { get; private set; }

        public EvaluationStack<StackEntry>? EntryStack;

        public BasicBlock(int offset)
        {
            StartOffset = offset;
            Label = LabelGenerator.GetLabel(LabelType.BasicBlock);
        }
    }
}
