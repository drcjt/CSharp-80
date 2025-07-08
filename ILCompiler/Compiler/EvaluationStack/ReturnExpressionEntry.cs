using ILCompiler.TypeSystem.Common;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class ReturnExpressionEntry : StackEntry
    {
        public CallEntry InlineCandidate { get; set; }

        // Represents the inline candidate's value. This is null
        // during the import that created the ReturnExpression
        // and is set later when the inline candidate is processed.
        public StackEntry? SubstitutionExpression { get; set; }

        public ReturnExpressionEntry(CallEntry inlineCandidate) : base(inlineCandidate.Type, inlineCandidate.Method!.Signature.ReturnType.GetElementSize().AsInt)
        {
            InlineCandidate = inlineCandidate;
        }

        public override StackEntry Duplicate()
        {
            throw new Exception("ReturnExpressionEntry should not be duplicated");
        }

        public override void Accept(IStackEntryVisitor visitor) => visitor.Visit(this);
    }
}
