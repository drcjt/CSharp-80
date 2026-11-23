using Internal.Runtime.CompilerServices;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
    public readonly ref struct ReadOnlySpan<T>
    {
        internal readonly ref T _reference;

        private readonly int _length;

        public ReadOnlySpan(T[]? array)
        {
            if (array == null)
            {
                this = default;
                return;
            }   

            _reference = ref MemoryMarshal.GetArrayDataReference(array);
            _length = array.Length;
        }

        public ReadOnlySpan(T[]? array, int start, int length)
        {
            if (array == null)
            {
                this = default;
                return;
            }

            _reference = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), start);
            _length = length;
        }
        
        public unsafe ReadOnlySpan(void* pointer, int length)
        {
            _reference = ref *(T*)pointer;
            _length = length;
        }

        internal ReadOnlySpan(ref T reference, int length)
        {
            _reference = ref reference;
            _length = length;
        }

        public ref readonly T this[int index]
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

        public static implicit operator ReadOnlySpan<T>(T[]? array) => new ReadOnlySpan<T>(array);

        public int Length => _length;

        public bool IsEmpty => _length == 0;

        public static ReadOnlySpan<T> Empty => default;

        // Spans can't be boxed, use operator == instead
        public override bool Equals(object? obj) => throw new NotSupportedException();
        public override int GetHashCode() => throw new NotSupportedException();

        public static bool operator ==(ReadOnlySpan<T> left, ReadOnlySpan<T> right) =>
            left._length == right._length && Unsafe.AreSame(ref left._reference, ref right._reference);

        public static bool operator !=(ReadOnlySpan<T> left, ReadOnlySpan<T> right) => !(left == right);

        /// <summary>
        /// Copies the contents of this read-only span into destination span. If the source
        /// and destinations overlap, this method behaves as if the original values in
        /// a temporary location before the destination is overwritten.
        /// </summary>
        /// <param name="destination">The span to copy items into.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the destination Span is shorter than the source Span.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(Span<T> destination)
        {
            // Using "if (!TryCopyTo(...))" results in two branches: one for the length
            // check, and one for the result of TryCopyTo. Since these checks are equivalent,
            // we can optimize by performing the check once ourselves then calling Memmove directly.

            if ((uint)_length <= (uint)destination.Length)
            {
                Buffer.Memmove(ref destination._reference, ref _reference, (uint)_length);
            }
            else
            {
                ThrowHelper.ThrowArgumentException_DestinationTooShort();
            }
        }

        /// <summary>
        /// Copies the contents of this read-only span into a new array.  This heap
        /// allocates, so should generally be avoided, however it is sometimes
        /// necessary to bridge the gap with APIs written in terms of arrays.
        /// </summary>
        public T[] ToArray()
        {
            if (IsEmpty)
            {
                return [];
            }

            var destination = new T[Length];
            CopyTo(destination);
            return destination;
        }
    }
}
