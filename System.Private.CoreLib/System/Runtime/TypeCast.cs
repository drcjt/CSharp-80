using Internal.Runtime;

namespace System.Runtime
{
    internal static class TypeCast
    {
        public static unsafe object IsInstanceOfClass(EEType* pTargetType, object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var pObjType = obj.m_pEEType;

            if (pObjType == pTargetType)
            {
                return obj;
            }

            do
            {
                pObjType = pObjType->RelatedType;
                if (pObjType == null)
                {
                    return null;
                }

                if (pObjType == pTargetType)
                {
                    return obj;
                }
            }
            while (true);
        }
    }
}
