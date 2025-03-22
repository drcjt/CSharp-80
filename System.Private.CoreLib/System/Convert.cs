namespace System
{
    public static class Convert
    {
        public static bool ToBoolean(int value) => value != 0;

        public static byte ToByte(int value) => ToByte((uint)value);
        public static byte ToByte(uint value)
        {
            if (value > byte.MaxValue) ThrowByteOverflowException();
            return (byte)value;
        }

        public static sbyte ToSByte(int value)
        {
            if (value < sbyte.MinValue || value > sbyte.MaxValue) ThrowSByteOverflowException();
            return (sbyte)value;
        }

        public static uint ToUInt32(int value)
        {
            if (value < 0) ThrowUInt32OverflowException();
            return (uint)value;
        }

        public static ushort ToUInt16(int value) => ToUInt16((uint)value);
        public static ushort ToUInt16(uint value)
        {
            if (value > ushort.MaxValue) ThrowUInt16OverflowException();
            return (ushort)value;
        }

        public static char ToChar(int value) => ToChar((uint)value);
        public static char ToChar(uint value)
        {
            if (value > char.MaxValue) ThrowCharOverflowException();
            return (char)value;
        }

        private static void ThrowCharOverflowException() { throw new OverflowException("Char"); }
        private static void ThrowByteOverflowException() { throw new OverflowException("Byte"); }
        private static void ThrowSByteOverflowException() { throw new OverflowException("SByte"); }
        private static void ThrowUInt16OverflowException() { throw new OverflowException("UInt16"); }
        private static void ThrowUInt32OverflowException() { throw new OverflowException("UInt32"); }
    }
}
