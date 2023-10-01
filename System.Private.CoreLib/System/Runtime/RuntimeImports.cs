using Internal.Runtime;
using System.Runtime.CompilerServices;

namespace System.Runtime
{
    public static class RuntimeImports
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        [RuntimeImport("NewString")]
        internal static unsafe extern string NewString(EETypePtr pEEType, int length);
    }
}
