namespace System.Collections
{
    public class Stack : ICollection
    {
        private object?[] _array;
        private int _size;

        private const int _defaultCapacity = 10;

        public Stack()
        {
            _array = new object[_defaultCapacity];
            _size = 0;
        }

        public Stack(int capacity)
        {
            if (capacity < _defaultCapacity)
                capacity = _defaultCapacity;

            _array = new object[capacity];
            _size = 0;
        }

        public virtual bool Contains(object? obj)
        {
            int count = _size;

            while (count-- > 0)
            {
                if (obj == null)
                {
                    if (_array[count] == null)
                        return true;
                }
                else if (_array[count] != null && _array[count]!.Equals(obj))
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(Array array, int index)
        {
            int i = 0;
            if (array is object?[] objArray)
            {
                while (i < _size)
                {
                    objArray[i + index] = _array[_size - i + 1];
                    i++;
                }
            }
            else
            {
                while (i < _size)
                {
                    array.SetValue(_array[_size - i + 1], i + index);
                    i++;
                }
            }
        }

        public virtual int Count => _size;

        public object? Peek()
        {
            if (_size == 0)
            {
                throw new InvalidOperationException("Stack is empty.");
            }

            return _array[_size - 1];
        }

        public  object? Pop()
        {
            if (_size == 0)
            {
                throw new InvalidOperationException("Stack is empty.");
            }

            object? value = _array[--_size];
            _array[_size] = null; // Clear the reference
            return value;
        }

        public void Push(object? value)
        {
            if (_size == _array.Length)
            {
                object[] newArray = new object[2 * _array.Length];
                Array.Copy(_array, newArray, _size);
                _array = newArray;
            }
            _array[_size++] = value;
        }

        public object?[] ToArray()
        {
            if (_size == 0)
                return [];

            var result = new object[_size];
            int i = 0;
            while (i < _size)
            {
                result[i] = _array[_size - i - 1]!;
                i++;
            }
            return result;
        }

        public IEnumerator GetEnumerator()
        {
            return new StackEnumerator(this);
        }

        private sealed class StackEnumerator : IEnumerator
        {
            private readonly Stack _stack;
            private int _index;
            private object? _currentElement;

            public StackEnumerator(Stack stack)
            {
                _stack = stack;
                _index = -2;
                _currentElement = null;
            }

            public object? Current
            {
                get
                {
                    if (_index == - 2)
                    {
                        throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");
                    }
                    if (_index == -1)
                    {
                        throw new InvalidOperationException("Enumeration already finished.");
                    }

                    return _currentElement;
                }
            }

            public bool MoveNext()
            {
                bool moreElements;
                if (_index == -2)
                {
                    _index = _stack._size - 1;
                    moreElements = _index >= 0;
                    if (moreElements)
                    {
                        _currentElement = _stack._array[_index];
                    }
                    return moreElements;
                }

                if (_index == -1)
                {
                    return false;
                }

                moreElements = --_index >= 0;
                if (moreElements)
                {
                    _currentElement = _stack._array[_index];
                }
                else
                {
                    _currentElement = null;
                }
                return moreElements;
            }

            public void Reset()
            {
                _index = _stack._size;
            }
        }
    }
}
