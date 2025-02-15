using System.Diagnostics;

namespace System.Collections.Generic
{
    internal struct ArrayBuilder<T>
    {
        private const int DefaultCapacity = 4;

        private T[] _array;
        private int _count;

        public ArrayBuilder(int capacity)
        {
            _array = new T[capacity];
        }

        public readonly int Capacity => _array == null ? 0 : _array.Length;

        public void Add(T item)
        {
            if (_count == Capacity)
                EnsureCapacity(_count + 1);

            _array[_count++] = item;
        }

        private void EnsureCapacity(int minimum)
        {
            Debug.Assert(minimum > Capacity);

            int capacity = Capacity;
            int nextCapacity = capacity == 0 ? DefaultCapacity : 2 * capacity;

            nextCapacity = Math.Min(nextCapacity, minimum);

            T[] next = new T[nextCapacity];
            if (_count > 0)
                Array.Copy(_array, next, _count);
            _array = next;
        }

        public readonly T[] ToArray()
        {   
            if (_count == 0)
            {
                return Array.Empty<T>();
            }

            T[] result = _array;

            if (_count < result.Length)
            {
                result = new T[_count];
                Array.Copy(_array, result, _count);
            }

            return result;
        }
    }
}