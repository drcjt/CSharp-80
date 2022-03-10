namespace ILCompiler.Common.TypeSystem.IL
{
    public enum StackValueKind
    {
        /// <summary>
        /// An unknown type
        /// </summary>
        Unknown,
        /// <summary>
        /// Any signed or unsigned integer values that can be represented in 32 bits
        /// </summary>
        Int32,
        /// <summary>
        /// Any signed or unsigned integer values that can be represented in 64 bits
        /// </summary>
        Int64,
        /// <summary>
        /// An unmanaged pointer type for the platform, for Z80, this is 16 bits
        /// </summary>
        NativeInt,
        /// <summary>
        /// Any float value
        /// </summary>
        Float,
        /// <summary>
        /// A managed pointer type for the platform, for Z80, this is 16 bits
        /// </summary>
        ByRef,
        /// <summary>
        /// An object reference
        /// </summary>
        ObjRef,
        /// <summary>
        /// A value type that is not one of the primitive ones
        /// </summary>
        ValueType
    }
}
