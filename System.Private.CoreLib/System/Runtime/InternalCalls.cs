using System.Runtime.InteropServices;

namespace System.Runtime
{
    internal static class InternalCalls
    {
        [DllImport(Libraries.Runtime, EntryPoint = "CALLCATCHHANDLER")]
        internal static extern unsafe void CallCatchHandler(object exception, byte* pCatchHandler, ref ExInfo exceptionInfo);

        [DllImport(Libraries.Runtime, EntryPoint = "EHENUMINIT")]
        internal static extern unsafe void EHEnumInit(void* pEHEnum);

        [DllImport(Libraries.Runtime, EntryPoint = "EHENUMNEXT")]
        internal static extern unsafe bool EHEnumNext(void* pEHEnum, void* pEHClause);
    }
}
