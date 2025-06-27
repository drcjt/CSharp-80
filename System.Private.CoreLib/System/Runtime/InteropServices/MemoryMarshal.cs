using Internal.Runtime.CompilerServices;
using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices
{
    public static unsafe class MemoryMarshal
    {
        public static ref T GetArrayDataReference<T>(T[] array) => ref Unsafe.As<byte, T>(ref Unsafe.As<RawArrayData>(array).Data);

        /// <summary>
        /// Returns a reference to the 0th element of <paramref name="array"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref byte GetArrayDataReference(Array array)
        {
            // Arrays are laid out in memory as follows:
            // [ pMethodTable || num components || array data .. ]
            //   ^               ^                 ^
            //   |               |                 \-- returned reference
            //   |               \-- ref Unsafe.As<RawData>(array).Data
            //   \-- array
            // The BaseSize of an array includes all the fields before the array data,
            // including the method table. The reference to RawData.Data
            // points at the number of components, skipping over the method table pointer-sized field.
            return ref Unsafe.AddByteOffset(ref Unsafe.As<RawData>(array).Data, (nint)array.GetMethodTable()->BaseSize - (1 * 2 /* sizeof(IntPtr) */));
        }

        public static ref T GetReference<T>(Span<T> span) => ref span._reference;

        public static ref T GetReference<T>(ReadOnlySpan<T> span) => ref span._reference;
    }
}
