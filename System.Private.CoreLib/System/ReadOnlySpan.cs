using Internal.Runtime.CompilerServices;
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

            // TODO: type mismatch check

            _reference = ref MemoryMarshal.GetArrayDataReference(array);
            _length = array.Length;
        }

        public ReadOnlySpan(T[]? array, int start, int length)
        {
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
                    throw new IndexOutOfRangeException();
                }

                return ref Unsafe.Add(ref _reference, index);
            }
        }

        public static implicit operator ReadOnlySpan<T>(T[]? array) => new ReadOnlySpan<T>(array);

        public int Length => _length;

        public bool IsEmpty => _length == 0;
    }
}
