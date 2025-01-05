using Internal.Runtime;
using Internal.Runtime.CompilerServices;

namespace System.Runtime
{
    internal static class RuntimeExports
    {
        internal static unsafe object Box(EEType* pEEType, ref byte data)
        {
            // Allocate box
            object result = InternalCalls.NewObject(pEEType);

            // Copy data into box
            Unsafe.CopyBlock(ref result.GetRawData(), ref data, pEEType->ValueTypeSize);

            return result;
        }
    }
}
