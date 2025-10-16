namespace ILCompiler.Compiler.EvaluationStack
{
    public class PhiArg : LocalVariableCommon
    {
        public BasicBlock PredecessorBlock { get; }

        public PhiArg(VarType type, int localNumber, int ssaNumber, BasicBlock block) : base(type)
        {
            LocalNumber = localNumber;
            SsaNumber = ssaNumber;
            PredecessorBlock = block;
        }

        public override StackEntry Duplicate()
        {
            return new PhiArg(Type, LocalNumber, SsaNumber, PredecessorBlock);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
