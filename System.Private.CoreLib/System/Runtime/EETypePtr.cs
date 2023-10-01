using Internal.Runtime;
using System.Runtime.CompilerServices;

namespace System
{
    internal unsafe struct EETypePtr
    {
        private EEType* _value;

        [Intrinsic]
        internal static EETypePtr EETypePtrOf<T>()
        {
            throw new Exception();
        }

        internal unsafe EEType* ToPointer()
        {
            return (EEType*)(void*)_value;
        }

        internal bool HasCctor
        {
            get
            {
                return _value->HasCctor;
            }
        }
    }
}
