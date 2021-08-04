﻿using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class ReturnEntry : StackEntry
    {
        public StackEntry Return { get; set; }

        public ReturnEntry() : base(StackValueKind.Unknown)
        {
            Operation = Operation.Return;
        }

        public ReturnEntry(StackEntry returnValue) : base(returnValue.Kind)
        {
            Return = returnValue;
        }
        public override StackEntry Duplicate()
        {
            return new ReturnEntry();
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
