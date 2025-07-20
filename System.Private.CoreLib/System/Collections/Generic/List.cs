namespace System.Collections.Generic
{
    public class List<T> : IList<T>
    {
        private const int DefaultCapacity = 4;

        internal T[] _items;
        internal int _size;

        public List()
        {
            _items = Array.Empty<T>();
        }

        public List(int capacity)
        {
            if (capacity == 0)
                _items = Array.Empty<T>();
            else
                _items = new T[capacity];
        }

        public List(IEnumerable<T> collection)
        {
            _items = Array.Empty<T>();

            IEnumerator<T> enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Add(enumerator.Current);
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
                        T[] newItems = new T[value];
                        if (_size > 0)
                        {
                            Array.Copy(_items, newItems, _size);
                        }

                        _items = newItems;
                    }
                    else
                    {
                        _items = new T[DefaultCapacity];
                    }
                }
            }
        }

        public int Count => _size;

        public T this[int index]
        {
            get
            {
                return _items[index];
            }
            set
            {
                _items[index] = value;
            }
        }

        public void Add(T item)
        {
            if (_size == _items.Length) { EnsureCapacity(_size + 1); }
            _items[_size++] = item;
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        private void EnsureCapacity(int min)
        {
            if (_items.Length < min)
            {
                int newCapacity = _items.Length == 0 ? DefaultCapacity : _items.Length * 2;
                if (newCapacity < min) newCapacity = min;
                Capacity = newCapacity;
            }
        }

        public void Clear()
        {
            if (_size > 0)
            {
                Array.Clear(_items, 0, _size);
                _size = 0;
            }
        }

        public bool Contains(T item) => IndexOf(item) >= 0;

        public int IndexOf(T item) => Array.IndexOf(_items, item, 0, _size);

        public void Insert(int index, T item)
        {
            if (_size == _items.Length) EnsureCapacity(_size + 1);
            if (index < _size)
            {
                Array.Copy(_items, index, _items, index + 1, _size - index);
            }
            _items[index] = item;
            _size++;
        }

        public void RemoveAt(int index)
        {
            _size--;
            if (index < _size)
            {
                Array.Copy(_items, index + 1, _items, index, _size - index);
            }
            _items[_size] = default!;
        }

        public IEnumerator<T> GetEnumerator() => new ListEnumeratorSimple(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void CopyTo(T[] array, int index)
        {
            throw new NotImplementedException();
        }

        private struct ListEnumeratorSimple : IEnumerator<T>
        {
            private readonly List<T> _list;
            private int _index;
            private T? _currentElement;

            internal ListEnumeratorSimple(List<T> list)
            {
                _list = list;
                _index = -1;
                _currentElement = default;
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
                    _currentElement = default;
                    return false;
                }
            }

            public readonly T Current => _currentElement!;

            readonly object? IEnumerator.Current => Current;

            public void Reset()
            {
                _currentElement = default;
                _index = -1;
            }

            public void Dispose() 
            { 
            }
        }
    }
}
