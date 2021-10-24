using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class ReturnEntry : StackEntry
    {
        public StackEntry? Return { get; set; }
        public int? ReturnBufferArgIndex { get; set; }
        public int? ReturnTypeExactSize { get; set; }

        public ReturnEntry() : this(null, null, null)
        {
        }

        public ReturnEntry(StackEntry? returnValue, int? returnBufferArgIndex, int? returnTypeExactSize) : base(returnValue?.Kind ?? StackValueKind.Unknown)
        {
            Operation = Operation.Return;
            Return = returnValue;
            ReturnBufferArgIndex = returnBufferArgIndex;
            ReturnTypeExactSize = returnTypeExactSize;
        }

        public override StackEntry Duplicate()
        {
            return new ReturnEntry(Return, ReturnBufferArgIndex, ReturnTypeExactSize);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
