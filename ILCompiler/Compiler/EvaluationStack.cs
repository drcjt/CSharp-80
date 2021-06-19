using ILCompiler.Common.TypeSystem.IL;
using System;
using System.Collections.Generic;

namespace ILCompiler.Compiler
{
    public class EvaluationStack<T>
    {
        private T[] _stack;
        private int _top;

        public EvaluationStack(int n)
        {
            _stack = new T[n];
            _top = 0;
        }

        public int Top 
        { 
            get 
            { 
                return _top;  
            } 
        }

        public int Length => Top;

        public void Push(T value)
        {
            if (_top >= _stack.Length)
            {
                Array.Resize(ref _stack, 2 * _top + 3);
            }
            _stack[_top++] = value;
        }

        public T this[int index]
        {
            get
            {
                return _stack[index];
            }

            set
            {
                _stack[index] = value;
            }
        }

        public T Peek()
        {
            // TODO: Deal with empty stack

            return _stack[_top - 1];
        }

        public T Pop()
        {
            // TODO: Deal with empty stack

            return _stack[--_top];
        }

        public void Clear()
        {
            _top = 0;
        }
    }
    
    public interface IVisitableStackEntry
    {
        public void Accept(IStackEntryVisitor visitor);
    }

    public interface IStackEntryVisitor
    {
        public void Visit(ConstantEntry entry);
        public void Visit(IndEntry entry);
        public void Visit(JumpTrueEntry entry);
        public void Visit(ReturnEntry entry);
        public void Visit(BinaryOperator entry);
        public void Visit(LocalVariableEntry entry);
        public void Visit(AssignmentEntry entry);
        public void Visit(CallEntry entry);
        public void Visit(IntrinsicEntry entry);

    }

    // StackEntry and subclasses represent the tree oriented high level intermediate representation
    // which will be the main output of the importer
    public abstract class StackEntry : IVisitableStackEntry
    {
        public StackValueKind Kind { get; }

        public StackEntry Next { get; set; }

        protected StackEntry(StackValueKind kind)
        {
            Kind = kind;
        }

        abstract public void Accept(IStackEntryVisitor visitor);
    }

    public abstract class ConstantEntry : StackEntry
    {
        protected ConstantEntry(StackValueKind kind) : base(kind)
        {
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
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

    // CNS_STR
    public class StringConstantEntry : ConstantEntry<String>
    {
        public StringConstantEntry(string value) : base(StackValueKind.ObjRef, value)
        {
        }
    }

    // CNS_INT
    public class Int16ConstantEntry : ConstantEntry<short>
    {
        public Int16ConstantEntry(short value) : base(StackValueKind.Int16, value)
        {
        }
    }

    // CNS_INT
    public class Int32ConstantEntry : ConstantEntry<int>
    {
        public Int32ConstantEntry(int value) : base(StackValueKind.Int32, value)
        {
        }
    }

    // STOREIND
    public class IndEntry : StackEntry
    {
        public StackEntry Addr { get; }
        public IndEntry(StackEntry addr) : base(addr.Kind)
        {
            Addr = addr;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    // JTRUE
    public class JumpTrueEntry : StackEntry
    {
        public StackEntry Condition { get; }
        public JumpTrueEntry(StackEntry condition) : base(StackValueKind.Unknown)
        {
            Condition = condition;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    // RETURN
    public class ReturnEntry : StackEntry
    {
        public StackEntry Return { get; set; }

        public ReturnEntry() : base(StackValueKind.Unknown)
        {
        }

        public ReturnEntry(StackEntry returnValue) : base(returnValue.Kind)
        {
            Return = returnValue;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public enum BinaryOp
    {
        ADD,
        SUB,
        MUL,
        EQ,
        NE,
        LT,
        LE,
        GT,
        GE
    }

    public class BinaryOperator : StackEntry
    {
        public BinaryOp Op { get; }
        public StackEntry Op1 { get; }
        public StackEntry Op2 { get; }

        public BinaryOperator(BinaryOp op, StackEntry op1, StackEntry op2, StackValueKind kind) : base(kind)
        {
            Op = op;
            Op1 = op1;
            Op2 = op2;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    // LCL_VAR
    public class LocalVariableEntry : StackEntry
    {
        public int LocalNumber { get; } 
        public LocalVariableEntry(int localNumber, StackValueKind kind) : base(kind)
        {
            LocalNumber = localNumber;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    // ASG
    public class AssignmentEntry : StackEntry
    {
        public StackEntry Op1 { get; }
        public StackEntry Op2 { get; }

        public AssignmentEntry(StackEntry op1, StackEntry op2) : base(op1.Kind)
        {
            Op1 = op1;
            Op2 = op2;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class CallEntry : StackEntry
    {
        public string TargetMethod { get; }
        public IList<StackEntry> Arguments;

        public CallEntry(string targetMethod, IList<StackEntry> arguments, StackValueKind returnKind) : base(returnKind)
        {
            TargetMethod = targetMethod;
            Arguments = arguments;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class IntrinsicEntry : StackEntry
    {
        public string TargetMethod { get; }
        public IList<StackEntry> Arguments;

        public IntrinsicEntry(string targetMethod, IList<StackEntry> arguments, StackValueKind returnKind) : base(returnKind)
        {
            TargetMethod = targetMethod;
            Arguments = arguments;
        }

        public override void Accept(IStackEntryVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    /* TODO: need to create classes to represent these HIR node types
    NOT,
    NOP,
    NEG,
    CAST,
    CMP,
    JCC,
    */
}