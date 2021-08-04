﻿namespace ILCompiler.Compiler.EvaluationStack
{
    public class StoreIndEntry : StackEntry
    {
        public StackEntry Addr { get; }
        public StackEntry Op1 { get; }

        public StoreIndEntry(StackEntry addr, StackEntry op1) : base(addr.Kind)
        {
            Operation = Operation.StoreIndirect;
            Addr = addr;
            Op1 = op1;
        }

        public override StackEntry Duplicate()
        {
            return new StoreIndEntry(Addr, Op1.Duplicate());
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
