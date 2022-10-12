namespace ILCompiler.Compiler.EvaluationStack
{
    public class SwitchEntry : StackEntry
    {
        public StackEntry Op1 { get; }
        public IList<string> JumpTable { get; }

        public SwitchEntry(StackEntry op1, IList<string> jumpTable) : base(VarType.Void)
        {
            Op1 = op1;
            JumpTable = jumpTable;
        }

        public override StackEntry Duplicate()
        {
            return new SwitchEntry(Op1, JumpTable);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
