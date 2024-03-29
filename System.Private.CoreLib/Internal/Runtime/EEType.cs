﻿using System.Runtime.CompilerServices;

namespace Internal.Runtime
{
    internal unsafe struct EEType
    {
        private ushort _usFlags;
        private ushort _usBaseSize;
        private EEType* _relatedType;

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
    }
}
