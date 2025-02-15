using Internal.Runtime;
using Internal.Runtime.CompilerServices;

namespace System.Runtime
{
    internal static class RuntimeExports
    {
        internal static unsafe object? Box(EEType* pEEType, ref byte data)
        {
            if (pEEType->IsNullable)
            {
                if (data == 0)
                    return null;

                // TODO: boxing of nullables
                // Need to determine type of nullable via generic parameter
                // Need to determine offset to actual data in nullable
                throw new NotImplementedException();
            }

            // Allocate box
            object result = InternalCalls.NewObject(pEEType);

            // Copy data into box
            Unsafe.CopyBlock(ref result.GetRawData(), ref data, pEEType->ValueTypeSize);

            return result;
        }

        internal static unsafe void Unbox(object? obj, ref byte data, EEType* pUnboxToEEType)
        {
            EEType* pEEType = obj.GetMethodTable();
            ref byte fields = ref obj.GetRawData();
            Unsafe.CopyBlock(ref data, ref fields, pEEType->ValueTypeSize);
        }
    }
}
