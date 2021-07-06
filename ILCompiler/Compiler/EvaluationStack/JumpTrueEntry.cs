﻿using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class JumpTrueEntry : StackEntry
    {
        public StackEntry Condition { get; }
        public string TargetLabel { get; }
        public JumpTrueEntry(string targetLabel, StackEntry condition) : base(StackValueKind.Unknown)
        {
            Operation = Operation.JumpTrue;
            TargetLabel = targetLabel;
            Condition = condition;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
