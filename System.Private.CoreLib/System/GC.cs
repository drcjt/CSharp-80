using System.Runtime;
using System.Runtime.CompilerServices;

namespace System
{
    public static class GC
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        [RuntimeImport("GCGetTotalMemory")]
        public static extern int GetTotalMemory();
    }
}
