using System.Runtime.CompilerServices;

namespace System.Runtime
{
    public static class RuntimeImports
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        [RuntimeImport("NewString")]
        public static unsafe extern string NewString(int length);
    }
}
