using System;

namespace GenericConstrainedCall
{
    internal static class ObjectInstanceMethodCaller<T>
    {
        public static RuntimeTypeHandle M(T t)
        {
            return t!.GetEEType();
        }
    }
}
