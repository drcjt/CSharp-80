using System.Diagnostics.CodeAnalysis;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class ReturnEntry : StackEntry
    {
        public StackEntry? Return { get; set; }
        public bool IsFinallyReturn { get; set; }
        public bool IsFilterReturn { get; set; }

        public ReturnEntry(StackEntry? returnValue, bool isFinallyReturn = false, bool isFilterReturn = false) : base(returnValue?.Type ?? VarType.Void)
        {
            Return = returnValue;
            IsFinallyReturn = isFinallyReturn;
            IsFilterReturn = isFilterReturn;
        }

        public override StackEntry Duplicate()
        {
            return new ReturnEntry(Return, IsFinallyReturn, IsFilterReturn);
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
