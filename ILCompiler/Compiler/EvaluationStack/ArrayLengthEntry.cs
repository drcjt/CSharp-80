using ILCompiler.Compiler.LinearIR;
using System.Diagnostics.CodeAnalysis;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class ArrayLengthEntry : StackEntry
    {
        public StackEntry ArrayReference { get; set; }
        public ArrayLengthEntry(StackEntry arrayReference) : base(VarType.Ptr, 2)
        {
            ArrayReference = arrayReference;
        }

        public override StackEntry Duplicate()
        {
            return new ArrayLengthEntry(ArrayReference);
        }

        public override void Accept(IStackEntryVisitor visitor) => visitor.Visit(this);

        public override bool TryGetUse(StackEntry operand, [NotNullWhen(true)] out Edge<StackEntry>? edge)
        {
            if (operand == ArrayReference)
            {
                edge = new Edge<StackEntry>(() => ArrayReference, x => ArrayReference = x);
                return true;
            }
            return base.TryGetUse(operand, out edge);
        }

    }
}
