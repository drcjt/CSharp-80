using System.Runtime.CompilerServices;
using Internal.Runtime.CompilerServices;

namespace Internal.Runtime
{
    internal unsafe struct EEType
    {
        // CS0649: Field '{blah}' is never assigned to, and will always have its default value
#pragma warning disable 649
        private ushort _usComponentSize;
        private ushort _usFlags;
        private ushort _usFlagsEx;
        private ushort _usBaseSize;
        private EEType* _relatedType;
        private byte _numVtableSlots;
        private byte _numInterfaces;
#pragma warning restore

        public readonly ushort GetFlags() { return _usFlags; }

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

        internal ushort ComponentSize => _usComponentSize;

        internal ushort BaseSize => _usBaseSize;

        internal bool IsValueType => ElementType < EETypeElementType.Class;

        internal bool IsInterface => ElementType == EETypeElementType.Interface;

        internal bool IsArray => ElementType == EETypeElementType.Array;

        internal EETypeElementType ElementType => (EETypeElementType)(_usFlags);

        // TODO: this should check FlagsEx
        internal bool IsNullable => ElementType == EETypeElementType.Nullable;

        internal uint ValueTypeSize => (uint)(BaseSize - 2);
    }
}
