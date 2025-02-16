using System;
using System.Runtime.CompilerServices;

namespace Internal.Runtime.CompilerServices
{
    public static unsafe partial class Unsafe
    {
        [Intrinsic]
        public static void* AsPointer<T>(ref T value)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // conv.u
            // ret
        }

        [Intrinsic]
        public static T As<T>(object? value) where T : class
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // ret
        }

        [Intrinsic]
        public static ref TTo As<TFrom, TTo>(ref TFrom source)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // ret
        }

        [Intrinsic]
        public static ref T AddByteOffset<T>(ref T source, IntPtr byteOffset)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // ldarg.1
            // add
            // ret
        }

        [Intrinsic]
        public static ref T AddByteOffset<T>(ref T source, nuint byteOffset)
        {
            return ref AddByteOffset(ref source, (IntPtr)(void*)byteOffset);

            // ldarg.0
            // ldarg.1
            // add
            // ret
        }

        /// <summary>
        /// Adds an element offset to the given reference.
        /// </summary>
        [Intrinsic]
        public static ref T Add<T>(ref T source, int elementOffset)
        {
            return ref AddByteOffset(ref source, elementOffset * (nint)sizeof(T));

            // ldarg .0
            // ldarg .1
            // sizeof !!T
            // conv.i
            // mul
            // add
            // ret
        }

        [Intrinsic]
        public static int SizeOf<T>()
        {
            return sizeof(T);
        }

        /// <summary>
        /// Reinterprets the given location as a reference to a value of type <typeparamref name="T"/>.
        /// </summary>
        [Intrinsic]
        public static ref T AsRef<T>(in T source)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // ret
        }

        [Intrinsic]
        public static void InitBlock(void* startAddress, byte value, uint byteCount)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // ldarg.1
            // ldarg.2
            // initblk
            // ret
        }

        [Intrinsic]
        public static void CopyBlock(void* destination, void* source, uint byteCount)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // ldarg.1
            // ldarg.2
            // cpblk
            // ret
        }

        [Intrinsic]
        public static void CopyBlock(ref byte destination, ref byte source, uint byteCount)
        {
            throw new PlatformNotSupportedException();

            // ldarg.0
            // ldarg.1
            // ldarg.2
            // cpblk
            // ret
        }
    }
}