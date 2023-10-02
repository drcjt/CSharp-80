using Internal.Runtime;
using Internal.Runtime.CompilerServices;

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
    }
}
