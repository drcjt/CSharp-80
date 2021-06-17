using ILCompiler.Common.TypeSystem.IL;
using System;

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

    /* TODO: Turn these into StackEntry subclasses
     * basically this will become the tree oriented high level intermediate representation
     * which will be the main output of the importer
    // Leaf Nodes
    LCL_VAR,
    STORE_LCL_VAR,

    // Constant Nodes
    CNS_INT,
    CNS_STR,

    // Unary Operators
    NOT,
    NOP,
    NEG,
    INTRINSIC,
    CAST,
    STOREIND,

    // Binary Operators
    ADD,
    SUB,
    MUL,
    ASG,
    EQ,
    NE,
    LT,
    LE,
    GE,
    GT,

    // Branching Operators
    CMP,
    JTRUE,
    JCC,

    // 
    CALL,
    RETURN,
    */


    // TODO: consider renaming this as this is effectively a GenTree node
    public abstract class StackEntry
    {
        public StackValueKind Kind { get; }

        protected StackEntry(StackValueKind kind)
        {
            Kind = kind;
        }
    }

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

    public class Int16ConstantEntry : ConstantEntry<short>
    {
        public Int16ConstantEntry(short value) : base(StackValueKind.Int16, value)
        {
        }
    }

    public class Int32ConstantEntry : ConstantEntry<int>
    {
        public Int32ConstantEntry(int value) : base(StackValueKind.Int32, value)
        {
        }
    }

    public class LocalVariableEntry : StackEntry
    {
        public LocalVariableEntry(StackValueKind kind) : base(kind)
        {
        }
    }

    public class AssignmentEntry : StackEntry
    {
        public StackEntry Op1 { get; }
        public StackEntry Op2 { get; }

        public AssignmentEntry(StackEntry op1, StackEntry op2) : base(op1.Kind)
        {
            Op1 = op1;
            Op2 = op2;
        }
    }

    public class ExpressionEntry : StackEntry
    {
        public ExpressionEntry(StackValueKind kind) : base(kind)
        {

        }
    }
}