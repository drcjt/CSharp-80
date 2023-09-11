namespace ILCompiler.Compiler.EvaluationStack
{
    public class PhiNode : StackEntry
    {
        public IList<PhiArg> Arguments { get; }

        public PhiNode(VarType type) : base(type)
        {
            Arguments = new List<PhiArg>();
        }

        public PhiNode(VarType type, IList<PhiArg> arguments) : base(type)
        {
            Arguments = arguments;
        }

        public override StackEntry Duplicate()
        {
            return new PhiNode(Type, Arguments);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
