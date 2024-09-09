using Internal.Runtime.CompilerServices;
using System.Runtime.CompilerServices;

namespace Internal.Runtime
{
    internal unsafe struct EEType
    {
        private ushort _usFlags;
        private ushort _usBaseSize;
        private EEType* _relatedType;
        private byte _numVtableSlots;
        private byte _numInterfaces;

        internal readonly ushort GetFlags() { return _usFlags; }

        internal readonly EEType* RelatedType => _relatedType;

        [Intrinsic]
        internal static extern EEType* Of<T>();

        internal bool HasCctor
        {
            get
            {
                return _usFlags == 1;
            }
        }

        internal byte NumVTableSlots => _numVtableSlots;
        internal byte NumInterfaces => _numInterfaces;

        internal EEType** InterfaceMap => (EEType**)((byte*)Unsafe.AsPointer(ref this) + sizeof(EEType) + sizeof(void*) * _numVtableSlots);
    }
}
