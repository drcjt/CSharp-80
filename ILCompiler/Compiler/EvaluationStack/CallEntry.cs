using ILCompiler.TypeSystem.Common;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class CallEntry : StackEntry
    {
        public string TargetMethod { get; }
        public IList<StackEntry> Arguments { get; }
        public bool IsVirtual { get; }

        public MethodDesc? Method { get; }

        public CallEntry(string targetMethod, IList<StackEntry> arguments, VarType returnType, int? returnSize, bool isVirtual = false, MethodDesc? method = null) : base(returnType, returnSize)
        {
            TargetMethod = targetMethod;
            Arguments = arguments;
            IsVirtual = isVirtual;
            Method = method;
        }

        public override StackEntry Duplicate()
        {
            return new CallEntry(TargetMethod, Arguments, Type, ExactSize, IsVirtual, Method);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
