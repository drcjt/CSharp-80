using ILCompiler.Common.TypeSystem.IL;
using System.Collections.Generic;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class CallEntry : StackEntry
    {
        public string TargetMethod { get; }
        public IList<StackEntry> Arguments;

        public CallEntry(string targetMethod, IList<StackEntry> arguments, StackValueKind returnKind, int? returnSize) : base(returnKind, returnSize)
        {
            Operation = Operation.Call;
            TargetMethod = targetMethod;
            Arguments = arguments;
        }

        public override StackEntry Duplicate()
        {
            return new CallEntry(TargetMethod, Arguments, Kind, ExactSize);
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
