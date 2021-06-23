﻿namespace ILCompiler.Compiler.EvaluationStack
{
    public interface IStackEntryVisitor
    {
        public void Visit(Int16ConstantEntry entry);
        public void Visit(Int32ConstantEntry entry);
        public void Visit(StringConstantEntry entry);
        public void Visit(StoreIndEntry entry);
        public void Visit(JumpTrueEntry entry);
        public void Visit(JumpEntry entry);
        public void Visit(ReturnEntry entry);
        public void Visit(BinaryOperator entry);
        public void Visit(LocalVariableEntry entry);
        public void Visit(StoreLocalVariableEntry entry);
        public void Visit(CallEntry entry);
        public void Visit(IntrinsicEntry entry);

    }
}