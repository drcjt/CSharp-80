using ILCompiler.Compiler.LinearIR;
using System.Diagnostics.CodeAnalysis;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class IntrinsicEntry : StackEntry
    {
        public string TargetMethod { get; }
        public IList<StackEntry> Arguments { get; }

        public IntrinsicEntry(string targetMethod, IList<StackEntry> arguments, VarType returnType) : base(returnType)
        {
            TargetMethod = targetMethod;
            Arguments = arguments;
        }

        public override StackEntry Duplicate()
        {
            return new IntrinsicEntry(TargetMethod, Arguments, Type);
        }

        public override void Accept(IStackEntryVisitor visitor) => visitor.Visit(this);

        public override bool TryGetUse(StackEntry operand, [NotNullWhen(true)] out Edge<StackEntry>? edge)
        {
            for (int i = 0; i < Arguments.Count; i++)
            {
                if (operand == Arguments[i])
                {
                    edge = new Edge<StackEntry>(() => Arguments[i], x => Arguments[i] = x);
                    return true;
                }
            }
            return base.TryGetUse(operand, out edge);
        }
    }
}
