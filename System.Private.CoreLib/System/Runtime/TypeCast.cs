using Internal.Runtime;

namespace System.Runtime
{
    internal static class TypeCast
    {
        public static unsafe object? IsInstanceOfInterface(EEType* pTargetType, object? obj)
        {
            if (obj == null)
            {
                return null;
            }

            var pObjType = obj.m_pEEType;
            byte interfaceCount = pObjType->NumInterfaces;

            EEType** interfaceMap = pObjType->InterfaceMap;

            while (interfaceCount > 0)
            {
                if (interfaceMap[0] == pTargetType)
                {
                    return obj;
                }

                interfaceMap++;
                interfaceCount--;
            }

            return null;
        }

        public static unsafe object? IsInstanceOfClass(EEType* pTargetType, object? obj)
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

        public static unsafe object? IsInstanceOfAny(EEType* pTargetType, object? obj)
        {
            throw new NotImplementedException();
        }

        public static unsafe object? CheckCastAny(EEType* pTargetType, object? obj)
        {
            throw new NotImplementedException();
        }

        public static unsafe object? CheckCastInterface(EEType* pTargetType, object? obj)
        {
            var result = IsInstanceOfInterface(pTargetType, obj);
            if (result != null)
                return obj;

            throw new InvalidCastException();
        }

        public static unsafe object? CheckCastClass(EEType* pTargetType, object? obj)
        {
            // Handle simple case where the object is null or already of the target type
            if (obj == null || obj.m_pEEType == pTargetType)
            {
                return obj;
            }

            return CheckCastClassSpecial(pTargetType, obj);
        }

        private static unsafe object? CheckCastClassSpecial(EEType* pTargetType, object? obj)
        {
            var result = IsInstanceOfClass(pTargetType, obj);
            if (result != null)
                return obj;

            throw new InvalidCastException();
        }
    }
}
