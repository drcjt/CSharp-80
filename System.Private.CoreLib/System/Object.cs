using Internal.Runtime;

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

        public virtual bool Equals(object? obj) => obj == this;
        public virtual int GetHashCode() => 0;
        public virtual string ToString() => "";

        public static bool ReferenceEquals(object? objA, object? objB) => objA == objB;
    }
}
