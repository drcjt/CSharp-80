namespace Internal.Runtime
{
    internal enum EETypeElementType
    {
        Unknown = 0x00,
        Void = 0x01,
        Boolean = 0x02,
        Char = 0x03,
        SByte = 0x04,
        Byte = 0x05,
        Int16 = 0x06,
        UInt16 = 0x07,
        Int32 = 0x08,
        UInt32 = 0x09,

        IntPtr = 0x0C,
        UIntPtr = 0x0D,

        ValueType = 0x10,
        // Enum = 0x11
        Nullable = 0x012,
        // Unused = 0x013,

        Class = 0x14,
        Interface = 0x15,

        SystemArray = 0x16, // System.Array

        Array = 0x17,
        SzArray = 0x18,
        ByRef = 0x19,
        Pointer = 0x1A,
        FunctionPointer = 0x1B,
    }
}
