namespace System.Collections
{
    public class ArrayList : IList
    {
        private object?[] _items;
        private int _size;

        private const int _defaultCapacity = 4;

        public ArrayList()
        {
            _items = new object[0]; // Array.Empty<object>();
        }

        public ArrayList(int capacity)
        {
            if (capacity == 0)
            {
                _items = new object[0]; // Array.Empty<object>();
            }
            else
            {
                _items = new object[capacity];
            }
        }

        public int Capacity
        {
            get => _items.Length;
            set
            {
                if (value != _items.Length)
                {
                    if (value > 0)
                    {
                        object[] newItems = new object[value];
                        if (_size > 0)
                        {
                            Array.Copy(_items, newItems, _size);
                        }
                        _items = newItems;
                    }
                    else
                    {
                        _items = new object[_defaultCapacity];
                    }
                }
            }
        }

        public virtual object? this[int index] 
        { 
            get => _items[index];
            set => _items[index] = value;
        }

        public int Count => _size;

        private void EnsureCapacity(int min)
        {
            if (_items.Length <  min)
            {
                int newCapacity = _items.Length == 0 ? _defaultCapacity : _items.Length * 2;
                if (newCapacity < min) newCapacity = min;
                Capacity = newCapacity;
            }
        }

        public virtual int Add(object? value)
        {
            if (_size == _items.Length) EnsureCapacity(_size + 1);
            _items[_size] = value;
            return _size++;
        }

        public void Clear()
        {
            if (_size > 0)
            {
                Array.Clear(_items, 0, _size);
                _size = 0;
            }
        }

        public bool Contains(object? value) => IndexOf(value) >= 0;

        public void CopyTo(Array array, int index)
        {
            //Array.Copy(_items, 0, array, index, _size);
            throw new System.NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            return new ArrayListEnumeratorSimple(this);
        }

        public int IndexOf(object? value) => Array.IndexOf(_items, value, 0, _size);

        public void Insert(int index, object? value)
        {
            if (_size == _items.Length) EnsureCapacity(_size + 1);
            if (index < _size)
            {
                Array.Copy(_items, index, _items, index + 1, _size - index);
            }
            _items[index] = value;
            _size++;
        }

        public void Remove(object? value)
        {
            int index = IndexOf(value);
            if (index >= 0)
            {
                RemoveAt(index);
            }
        }

        public void RemoveAt(int index)
        {
            _size--;
            if (index < _size)
            {
                Array.Copy(_items, index + 1, _items, index, _size - index);
            }
            _items[_size] = null;
        }

        private struct ArrayListEnumeratorSimple : IEnumerator
        {
            private readonly ArrayList _list;
            private int _index;
            private object? _currentElement;
            private static readonly object _dummyObject = new object();
            internal ArrayListEnumeratorSimple(ArrayList list)
            {
                _list = list;
                _index = -1;
                _currentElement = _dummyObject;
            }

            public bool MoveNext()
            {
                if (_index < _list.Count - 1)
                {
                    _currentElement = _list[++_index];
                    return true;
                }
                else
                {
                    _index = _list.Count;
                    _currentElement = _dummyObject;
                    return false;
                }
            }

            public readonly object? Current => _currentElement;

            public void Reset()
            {
                _currentElement = _dummyObject;
                _index = -1;
            }
        }
    }
}
