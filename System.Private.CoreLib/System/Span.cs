using Internal.Runtime.CompilerServices;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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

        // Constructor for internal use only. It is not safe to expose publicly, and is instead exposed via the unsafe MemoryMarshal.CreateSpan.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Span(ref T reference, int length)
        {
            Debug.Assert(length >= 0);

            _reference = ref reference;
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

        // Spans can't be boxed, use operator == instead
        public override bool Equals(object? obj) => throw new NotSupportedException();
        public override int GetHashCode() => throw new NotSupportedException();

        public static bool operator ==(Span<T> left, Span<T> right) =>
            left._length == right._length && Unsafe.AreSame(ref left._reference, ref right._reference);

        public static bool operator !=(Span<T> left, Span<T> right) => !(left == right);

        /// <summary>
        /// Clears the contents of this span.
        /// </summary>
        public unsafe void Clear()
        {
            SpanHelpers.ClearWithoutReferences(ref Unsafe.As<T, byte>(ref _reference), (uint)_length * (nuint)sizeof(T));
        }

        /// <summary>
        /// Forms a slice out of the given span, beginning at 'start'.
        /// </summary>
        /// <param name="start">The zero-based index at which to begin this slice.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the specified <paramref name="start"/> index is not in range (&lt;0 or &gt;Length).
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> Slice(int start)
        {
            if ((uint)start > (uint)_length)
                ThrowHelper.ThrowArgumentOutOfRangeException();

            return new Span<T>(ref Unsafe.Add(ref _reference, (nint)(uint)start /* force zero-extension */), _length - start);
        }

        /// <summary>
        /// Copies the contents of this span into a new array.  This heap
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

        /// <summary>
        /// Copies the contents of this span into destination span. If the source
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
    }
}
