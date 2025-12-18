using System.Runtime.CompilerServices;

namespace System.Diagnostics
{
    public static class Debugger
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Break()
        {
            Debug.DebugBreak();
        }
    }
}
