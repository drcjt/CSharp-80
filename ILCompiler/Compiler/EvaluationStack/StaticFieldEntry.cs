namespace ILCompiler.Compiler.EvaluationStack
{
    public class StaticFieldEntry : StackEntry
    {
        public string Name { get; }

        public StaticFieldEntry(String name) : base(VarType.Ptr, 2)
        {
            Name = name;
        }

        public override StaticFieldEntry Duplicate()
        {
            return new StaticFieldEntry(Name);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
