namespace ILCompiler.Compiler.EvaluationStack
{
    /// <summary>
    /// Represents an expression by a string
    /// </summary>
    public class ExpressionEntry : StackEntry
    {
        /// <summary>
        /// String representation of the expression
        /// </summary>
        public string Name { get; }

        public ExpressionEntry(VarType type, String name) : base(type, type.GetTypeSize())
        {
            Name = name;
        }

        public override ExpressionEntry Duplicate()
        {
            return new ExpressionEntry(Type, Name);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
