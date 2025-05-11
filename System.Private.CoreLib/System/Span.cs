using Internal.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
    public readonly ref struct Span<T>
    {
        internal readonly ref T _reference;

        private readonly int _length;

        public Span(T[]? array)
        {
            if (array == null)
            {
                this = default;
                return;
            }   

            // TODO: type mismatch check

            _reference = ref MemoryMarshal.GetArrayDataReference(array);
            _length = array.Length;
        }

        public Span(T[]? array, int start, int length)
        {
            _reference = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), start);
            _length = length;
        }
        
        public unsafe Span(void* pointer, int length)
        {
            _reference = ref *(T*)pointer;
            _length = length;
        }

        public ref T this[int index]
        {
            get
            {
                if ((uint)index >= (uint)_length)
                {
                    ThrowHelper.ThrowIndexOutOfRangeException();
                }

                return ref Unsafe.Add(ref _reference, index);
            }
        }

        public static implicit operator Span<T>(T[]? array) => new Span<T>(array);

        public static implicit operator ReadOnlySpan<T>(Span<T> span) => new ReadOnlySpan<T>(ref span._reference, span._length);

        public int Length => _length;

        public bool IsEmpty => _length == 0;

        public static Span<T> Empty => default;

        public static bool operator ==(Span<T> left, Span<T> right) =>
            left._length == right._length && Unsafe.AreSame(ref left._reference, ref right._reference);

        public static bool operator !=(Span<T> left, Span<T> right) => !(left == right);
    }
}
