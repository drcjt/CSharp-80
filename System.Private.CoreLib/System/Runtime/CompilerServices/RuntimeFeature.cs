namespace System.Runtime.CompilerServices
{
    public static class RuntimeFeature
    {
        public const string DefaultImplementationsOfInterfaces = nameof(DefaultImplementationsOfInterfaces);
        public const string NumericIntPtr = nameof(NumericIntPtr);
        public const string ByRefFields = nameof(ByRefFields);

        public static bool IsSupported(string feature)
        {
            return true;
        }
    }
}
