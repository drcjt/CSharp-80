using dnlib.DotNet;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class CallEntry : StackEntry
    {
        public string TargetMethod { get; }
        public IList<StackEntry> Arguments { get; }

        public bool IsInternalCall { get; }
        public bool IsVirtual { get; }

        public MethodDef? Method { get; }

        public CallEntry(string targetMethod, IList<StackEntry> arguments, VarType returnType, int? returnSize, bool isInternalCall = false, bool isVirtual = false, MethodDef? method = null) : base(returnType, returnSize)
        {
            TargetMethod = targetMethod;
            Arguments = arguments;
            IsInternalCall = isInternalCall;
            IsVirtual = isVirtual;
            Method = method;
        }

        public override StackEntry Duplicate()
        {
            return new CallEntry(TargetMethod, Arguments, Type, ExactSize, IsInternalCall, IsVirtual, Method);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
