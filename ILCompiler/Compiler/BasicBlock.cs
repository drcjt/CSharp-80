using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.Ssa;

namespace ILCompiler.Compiler
{
    public enum JumpKind
    {
        Return,         // block ends with ret
        Always,         // block always jumps to the target
        Conditional,    // block conditionally jumps to target
        Switch,         // block ends with a switch statement
    }

    public class BasicBlock
    {
        public BasicBlock? Next { get; set; }
        public int StartOffset { get; set; }
        public int EndOffset { get; set; }

        public JumpKind JumpKind { get; set; }

        public uint PostOrderNum { get; set; }

        public BasicBlock? ImmediateDominator { get; set; }
        public IList<BasicBlock> Successors { get; } = new List<BasicBlock>();
        public IList<BasicBlock> Predecessors { get; } = new List<BasicBlock>();
        public ISet<BasicBlock> Reach { get; } = new HashSet<BasicBlock>();

        // High level intermediate representation - main output of importation process
        public IList<StackEntry> Statements { get; } = new List<StackEntry>();

        public StackEntry? FirstNode { get; set; }
        public bool Marked { get; set; } = false;

        public string Label { get; private set; }

        public EvaluationStack<StackEntry>? EntryStack { get; set; }

        // Variables used by the block, before a definition
        public VariableSet VarUse { get; set; } = VariableSet.Empty;

        // Variables assigned by the block, before a use
        public VariableSet VarDef { get; set; } = VariableSet.Empty;

        // Variables live on entry to the block
        public VariableSet LiveIn { get; set; } = VariableSet.Empty;

        // Variables live on exit from the block
        public VariableSet LiveOut { get; set; } = VariableSet.Empty;

        // Variables in scope over the block
        public VariableSet Scope { get; set; } = VariableSet.Empty;

        public bool TryStart { get; set; }
        public bool FilterStart { get; set; }
        public bool HandlerStart { get; set; }

        public IList<BasicBlock> TryBlocks { get; set; } = new List<BasicBlock>();

        public IList<BasicBlock> Handlers { get; set; } = new List<BasicBlock>();

        public BasicBlock(int offset)
        {
            StartOffset = offset;
            Label = LabelGenerator.GetLabel(LabelType.BasicBlock);
            JumpKind = JumpKind.Always;
        }
    }
}
