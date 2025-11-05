using Internal.Runtime;
using System.Runtime.CompilerServices;

namespace System.Runtime
{
    public static class RuntimeImports
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        [RuntimeImport("NewString")]
        internal static unsafe extern string NewString(EEType* pEEType, nuint length);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [RuntimeImport("NewArray")]
        internal static unsafe extern Array NewArray(EEType* pEEType, nuint length);
    }
}
