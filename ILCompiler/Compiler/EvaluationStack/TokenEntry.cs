using ILCompiler.TypeSystem.Common;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class TokenEntry : StackEntry
    {
        public FieldDesc Field { get; }
        public string Label { get; }
        public TokenEntry(FieldDesc field, string label) : base(VarType.Void)
        {
            Field = field;
            Label = label;
        }

        public override StackEntry Duplicate()
        {
            return new TokenEntry(Field, Label);
        }

        public override void Accept(IStackEntryVisitor visitor) => visitor.Visit(this);
    }
}
