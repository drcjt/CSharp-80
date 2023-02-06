using System.Runtime.InteropServices;

namespace System.Runtime
{
    public static class RuntimeImports
    {

        [DllImport(Libraries.Runtime, EntryPoint = "NewString")]
        public static unsafe extern string NewString(int length);
    }
}
