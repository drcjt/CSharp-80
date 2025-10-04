namespace ILCompiler.Compiler.EvaluationStack
{
    public class PhiArg : LocalVariableCommon
    {
        public BasicBlock Block { get; }

        public PhiArg(VarType type, int localNumber, int ssaNumber, BasicBlock block) : base(type)
        {
            LocalNumber = localNumber;
            SsaNumber = ssaNumber;
            Block = block;
        }

        public override StackEntry Duplicate()
        {
            return new PhiArg(Type, LocalNumber, SsaNumber, Block);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
