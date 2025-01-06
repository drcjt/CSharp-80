using Internal.Runtime;
using Internal.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
    public unsafe class Object
    {
        internal EEType* m_pEEType;

        // Equivalent to GetType().TypeHandle
        public RuntimeTypeHandle GetEEType()
        {
            return new RuntimeTypeHandle((IntPtr)m_pEEType);
        }

        public virtual bool Equals(object? obj) => this == obj;

        public static bool Equals(object? obj1, object? obj2)
        {
            if (obj1 == obj2) return true;
            if (obj1 == null || obj2 == null) return false;
            return obj1.Equals(obj2);
        }

        public virtual int GetHashCode() => 0;
        public virtual string ToString() => "";

        public static bool ReferenceEquals(object? objA, object? objB) => objA == objB;

        internal EEType* GetMethodTable() => m_pEEType;

        [StructLayout(LayoutKind.Sequential)]
        private sealed class RawData
        {
            public byte Data;
        }

        internal ref byte GetRawData()
        {
            return ref Unsafe.As<RawData>(this).Data;
        }
    }
}
