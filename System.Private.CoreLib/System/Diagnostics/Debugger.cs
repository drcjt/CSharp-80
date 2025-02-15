using System.Runtime;
using System.Runtime.CompilerServices;

namespace System.Diagnostics
{
    public static class Debugger
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        [RuntimeImport("InvokeSystemDebugger")]
        public static void Break()
        {
        }
    }
}
