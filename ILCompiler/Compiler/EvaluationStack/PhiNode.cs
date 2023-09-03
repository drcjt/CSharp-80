namespace ILCompiler.Compiler.EvaluationStack
{
    public class PhiNode : StackEntry
    {
        public PhiNode(VarType type) : base(type)
        {
        }

        public override StackEntry Duplicate()
        {
            return new PhiNode(Type);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
