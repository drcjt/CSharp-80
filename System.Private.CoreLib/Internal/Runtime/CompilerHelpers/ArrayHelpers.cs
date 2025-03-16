using System;

namespace Internal.Runtime.CompilerHelpers
{
    internal static class ArrayHelpers
    {
        public static unsafe Array NewObjArray(EEType* pEEType, int nDimensions, int* pDimensions)
        {
            return Array.NewMultiDimArray(pEEType, pDimensions, nDimensions);
        }
    }
}
