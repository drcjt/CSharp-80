using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        private abstract partial class Iterator<TSource> : IEnumerable<TSource>, IEnumerator<TSource>
        {
            private protected int _state;
            private protected TSource _current = default!;

            public TSource Current => _current;

            private protected abstract Iterator<TSource> Clone();

            public virtual void Dispose()
            {
                _current = default!;
                _state = -1;
            }

            public Iterator<TSource> GetEnumerator()
            {
                Iterator<TSource> enumerator = _state == 0 ? this : Clone();
                enumerator._state = 1;
                return enumerator;
            }

            public abstract bool MoveNext();

            object? IEnumerator.Current => Current;

            IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            void IEnumerator.Reset() => ThrowHelper.ThrowNotSupportedException();
        }
    }
}
