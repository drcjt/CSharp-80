using System.Diagnostics.CodeAnalysis;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class ReturnEntry : StackEntry
    {
        public StackEntry? Return { get; set; }

        public ReturnEntry(StackEntry? returnValue) : base(returnValue?.Type ?? VarType.Void)
        {
            Return = returnValue;
        }

        public override StackEntry Duplicate()
        {
            return new ReturnEntry(Return);
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
