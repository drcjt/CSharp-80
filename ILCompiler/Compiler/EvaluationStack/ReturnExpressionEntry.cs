using ILCompiler.TypeSystem.Common;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class ReturnExpressionEntry : StackEntry
    {
        public CallEntry InlineCandidate { get; set; }
        public StackEntry? SubstExpr { get; set; } = null;

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
