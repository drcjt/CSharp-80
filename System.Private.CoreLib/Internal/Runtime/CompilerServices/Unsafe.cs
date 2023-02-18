using System;
using System.Runtime.CompilerServices;

namespace Internal.Runtime.CompilerServices
{
    public static unsafe partial class Unsafe
    {
        [Intrinsic]
        public static void* AsPointer<T>(ref T value)
        {
            throw new Exception();

            // ldarg.0
            // conv.u
            // ret
        }

        [Intrinsic]
        public static ref T AddByteOffset<T>(ref T source, IntPtr byteOffset)
        {
            throw new Exception();

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
            throw new Exception();
        }

        [Intrinsic]
        public static int SizeOf<T>()
        {
            throw new Exception();

            // sizeof !!0
            // ret
        }

        /// <summary>
        /// Reinterprets the given location as a reference to a value of type <typeparamref name="T"/>.
        /// </summary>
        [Intrinsic]
        public static ref T AsRef<T>(in T source)
        {
            throw new Exception();
        }
    }
}
