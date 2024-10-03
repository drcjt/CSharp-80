using ILCompiler.Compiler.LinearIR;
using System.Diagnostics.CodeAnalysis;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class ReturnEntry : StackEntry
    {
        public StackEntry? Return { get; set; }
        public int? ReturnBufferArgIndex { get; }
        public int? ReturnTypeExactSize { get; }

        public ReturnEntry() : this(null, null, null)
        {
        }

        public ReturnEntry(StackEntry? returnValue, int? returnBufferArgIndex, int? returnTypeExactSize) : base(returnValue?.Type ?? VarType.Void)
        {
            Return = returnValue;
            ReturnBufferArgIndex = returnBufferArgIndex;
            ReturnTypeExactSize = returnTypeExactSize;
        }

        public override StackEntry Duplicate()
        {
            return new ReturnEntry(Return, ReturnBufferArgIndex, ReturnTypeExactSize);
        }

        public override void Accept(IStackEntryVisitor visitor) => visitor.Visit(this);

        public override bool TryGetUse(StackEntry operand, [NotNullWhen(true)] out Edge<StackEntry>? edge)
        {
            edge = null;

            if (operand == Return)
            {
                edge = new Edge<StackEntry>(() => Return, x => Return = x);
                return true;
            }
            return false;
        }
    }
}
