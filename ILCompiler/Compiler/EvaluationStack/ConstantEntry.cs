﻿using ILCompiler.Common.TypeSystem.IL;
using System;

namespace ILCompiler.Compiler.EvaluationStack
{
    public abstract class ConstantEntry : StackEntry
    {
        protected ConstantEntry(StackValueKind kind) : base(kind)
        {
        }
    }

    public abstract class ConstantEntry<T> : ConstantEntry where T : IConvertible
    {
        public T Value { get; }

        protected ConstantEntry(StackValueKind kind, T value) : base(kind)
        {
            Value = value;
        }
    }

    public class StringConstantEntry : ConstantEntry<String>
    {
        public string Label { get; set; }
        public StringConstantEntry(string value) : base(StackValueKind.ObjRef, value)
        {
            Operation = Operation.Constant_String;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class Int16ConstantEntry : ConstantEntry<short>
    {
        public Int16ConstantEntry(short value) : base(StackValueKind.Int16, value)
        {
            Operation = Operation.Constant_Int16;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class Int32ConstantEntry : ConstantEntry<int>
    {
        public Int32ConstantEntry(int value) : base(StackValueKind.Int32, value)
        {
            Operation = Operation.Constant_Int32;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
