﻿using ILCompiler.Compiler.LinearIR;
using System.Diagnostics.CodeAnalysis;

namespace ILCompiler.Compiler.EvaluationStack
{
    public class CastEntry : StackEntry
    {
        public StackEntry Op1 { get; set; }

        public CastEntry(StackEntry op1, VarType type) : base(type, type.GetTypeSize() /*op1.ExactSize */)
        {
            Op1 = op1;
        }

        public override StackEntry Duplicate()
        {
            return new CastEntry(Op1.Duplicate(), Type);
        }

        public override void Accept(IStackEntryVisitor visitor) => visitor.Visit(this);

        public override bool TryGetUse(StackEntry operand, [NotNullWhen(true)] out Edge<StackEntry>? edge)
        {
            if (operand == Op1)
            {
                edge = new Edge<StackEntry>(() => Op1, x => Op1 = x);
                return true;
            }
            return base.TryGetUse(operand, out edge);
        }
    }
}
