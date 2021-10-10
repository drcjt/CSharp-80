using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class ReturnEntry : StackEntry
    {
        public StackEntry Return { get; set; }
        public int? ReturnBufferArgIndex { get; set; }
        public int? ReturnTypeExactSize { get; set; }

        public ReturnEntry() : base(StackValueKind.Unknown)
        {
            Operation = Operation.Return;
        }

        public ReturnEntry(StackEntry returnValue, int? returnBufferArgIndex, int? returnTypeExactSize) : base(returnValue.Kind)
        {
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
