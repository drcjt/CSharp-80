namespace System.Runtime.CompilerServices
{
    public static class RuntimeFeature
    {
        public const string DefaultImplementationsOfInterfaces = nameof(DefaultImplementationsOfInterfaces);

        public static bool IsSupported(string feature)
        {
            return true;
        }
    }
}
