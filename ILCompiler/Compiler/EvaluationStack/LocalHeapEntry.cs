﻿using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class LocalHeapEntry : StackEntry
    {
        public StackEntry Op1 { get; }

        public LocalHeapEntry(StackEntry op1, StackValueKind objKind) : base(objKind)
        {
            Op1 = op1;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override StackEntry Duplicate()
        {
            return new LocalHeapEntry(Op1, Kind);
        }
    }
}
