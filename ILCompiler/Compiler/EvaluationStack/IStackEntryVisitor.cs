﻿namespace ILCompiler.Compiler.EvaluationStack
{
    public interface IGenericStackEntryVisitor
    {
        public void Visit<T>(T entry) where T : StackEntry;
    }

    public interface IStackEntryVisitor
    {
        public void Visit(NativeIntConstantEntry entry);
        public void Visit(Int32ConstantEntry entry);
        public void Visit(StoreIndEntry entry);
        public void Visit(JumpTrueEntry entry);
        public void Visit(JumpEntry entry);
        public void Visit(ReturnEntry entry);
        public void Visit(BinaryOperator entry);
        public void Visit(LocalVariableEntry entry);
        public void Visit(LocalVariableAddressEntry entry);
        public void Visit(StoreLocalVariableEntry entry);
        public void Visit(CallEntry entry);
        public void Visit(IntrinsicEntry entry);
        public void Visit(CastEntry entry);
        public void Visit(UnaryOperator entry);
        public void Visit(IndirectEntry entry);
        public void Visit(FieldAddressEntry entry);
        public void Visit(SwitchEntry entry);
        public void Visit(AllocObjEntry entry);
        public void Visit(LocalHeapEntry entry);
        public void Visit(IndexRefEntry entry);
        public void Visit(PutArgTypeEntry entry);
        public void Visit(SymbolConstantEntry entry);
        public void Visit(CommaEntry entry);
        public void Visit(PhiNode entry);
        public void Visit(PhiArg entry);
        public void Visit(BoundsCheck entry);
        public void Visit(NullCheckEntry entry);
        public void Visit(CatchArgumentEntry entry);
        public void Visit(TokenEntry entry);
        public void Visit(ArrayLengthEntry entry);
        public void Visit(ReturnExpressionEntry entry);
        public void Visit(NothingEntry entry);
    }
}
