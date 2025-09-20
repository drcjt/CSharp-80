using ILCompiler.Compiler.Inlining;
using ILCompiler.TypeSystem.Common;
using System.Diagnostics.CodeAnalysis;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class CallEntry : StackEntry
    {
        public string TargetMethod { get; }
        public IList<StackEntry> Arguments { get; }
        public bool IsVirtual { get; }

        public bool IsInlineCandidate { get; set; }
        public InlineInfo? InlineInfo { get; set; } = null;

        public InlineCandidateInfo? InlineCandidateInfo{ get ;set; } = null;
        public MethodDesc? Method { get; }

        public bool HasReturnBuffer { get; set; }

        public void SetReturnType(VarType type) => Type = type;

        public CallEntry(string targetMethod, IList<StackEntry> arguments, VarType returnType, int? returnSize, bool isVirtual = false, MethodDesc? method = null, bool isInlineCandidate = false) : base(returnType, returnSize)
        {
            TargetMethod = targetMethod;
            Arguments = arguments;
            IsVirtual = isVirtual;
            Method = method;
            IsInlineCandidate = isInlineCandidate;
         }

        public override StackEntry Duplicate()
        {
            var duplicate = new CallEntry(TargetMethod, Arguments, Type, ExactSize, IsVirtual, Method, IsInlineCandidate);
            duplicate.HasReturnBuffer = HasReturnBuffer;
            duplicate.InlineInfo = InlineInfo;
            duplicate.InlineCandidateInfo = InlineCandidateInfo;
            return duplicate;
        }

        public override void Accept(IStackEntryVisitor visitor) => visitor.Visit(this);

        public override bool TryGetUse(StackEntry operand, [NotNullWhen(true)] out Edge<StackEntry>? edge)
        {
            for (int i = 0; i < Arguments.Count; i++)
            {
                if (Arguments[i] == operand)
                {
                    edge = new Edge<StackEntry>(() => Arguments[i], x => Arguments[i] = x);
                    return true;
                }
            }
            return base.TryGetUse(operand, out edge);
        }
    }
}
