using Internal.Runtime;
using System.Runtime.CompilerServices;

namespace System
{
    public unsafe struct EETypePtr
    {
        [Intrinsic]
        internal static EEType* EETypePtrOf<T>()
        {
            throw new Exception();
        }
    }
}
