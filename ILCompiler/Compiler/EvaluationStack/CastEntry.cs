﻿using ILCompiler.Common.TypeSystem;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class CastEntry : StackEntry
    {
        public WellKnownType DesiredType { get; }
        public StackEntry Op1 { get; }

        public bool Unsigned { get; }

        public CastEntry(WellKnownType desiredType, bool unsigned, StackEntry op1) : base(op1.Kind, op1.ExactSize)
        {
            Operation = Operation.Cast;
            DesiredType = desiredType;
            Op1 = op1;
            Unsigned = unsigned;
        }

        public override StackEntry Duplicate()
        {
            return new CastEntry(DesiredType, Unsigned, Op1.Duplicate());
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
