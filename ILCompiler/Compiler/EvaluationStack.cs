using ILCompiler.Common.TypeSystem.IL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public class ExpressionEntry : StackEntry
    {
        public ExpressionEntry(StackValueKind kind) : base(kind)
        {

        }
    }
}