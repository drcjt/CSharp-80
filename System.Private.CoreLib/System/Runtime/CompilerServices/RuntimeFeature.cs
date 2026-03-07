namespace System.Runtime.CompilerServices
{
    public static class RuntimeFeature
    {
        /// <summary>
        /// Indicates that this version of runtime supports default interface method implementations.
        /// </summary>
        public const string DefaultImplementationsOfInterfaces = nameof(DefaultImplementationsOfInterfaces);

        /// <summary>
        /// Indicates that this version of runtime supports <see cref="System.IntPtr" /> and <see cref="System.UIntPtr" /> as numeric types.
        /// </summary>
        public const string NumericIntPtr = nameof(NumericIntPtr);

        /// <summary>
        /// Represents a runtime feature where types can define ref fields.
        /// </summary>
        public const string ByRefFields = nameof(ByRefFields);

        /// <summary>
        /// Checks whether a certain feature is supported by the Runtime.
        /// </summary>
        public static bool IsSupported(string feature)
        {
            return feature switch
            {
                ByRefFields or
                DefaultImplementationsOfInterfaces or
                NumericIntPtr => true,

                nameof(IsMultithreadingSupported) => IsMultithreadingSupported,
                _ => false,
            };
        }

        /// <summary>
        /// Gets a value that indicates whether the runtime supports multithreading, including
        /// creating threads and using blocking synchronization primitives. This property
        /// returns <see langword="false"/> on platforms or configurations where multithreading
        /// is not supported or is disabled.
        /// </summary>
        public static bool IsMultithreadingSupported => false;

        internal static void ThrowIfMultithreadingIsNotSupported()
        {
            throw new PlatformNotSupportedException();
        }
    }
}
