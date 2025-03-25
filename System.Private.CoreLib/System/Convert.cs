namespace System
{
    public static class Convert
    {
        public static bool ToBoolean(ushort value) => value != 0;
        public static bool ToBoolean(uint value) => value != 0;
        public static bool ToBoolean(int value) => value != 0;
        public static bool ToBoolean(byte value) => value != 0;

        public static byte ToByte(byte value) => value;
        public static byte ToByte(bool value) => value ? (byte)bool.True : (byte)bool.False;
        public static byte ToByte(int value) => ToByte((uint)value);
        public static byte ToByte(uint value)
        {
            if (value > byte.MaxValue) ThrowByteOverflowException();
            return (byte)value;
        }
        public static byte ToByte(char value)
        {
            if (value > byte.MaxValue) ThrowByteOverflowException();
            return (byte)value;
        }
        public static byte ToByte(sbyte value)
        {
            if (value < 0) ThrowByteOverflowException();
            return (byte)value;
        }
        public static byte ToByte(ushort value)
        {
            if (value > byte.MaxValue) ThrowByteOverflowException();
            return (byte)value;
        }

        public static sbyte ToSByte(sbyte value) => value;
        public static sbyte ToSByte(bool value) => value ? (sbyte)bool.True : (sbyte)bool.False;
        public static sbyte ToSByte(int value)
        {
            if (value < sbyte.MinValue || value > sbyte.MaxValue) ThrowSByteOverflowException();
            return (sbyte)value;
        }
        public static sbyte ToSByte(byte value)
        {
            if (value > sbyte.MaxValue) ThrowSByteOverflowException();
            return (sbyte)value;
        }
        public static sbyte ToSByte(char value)
        {
            if (value > sbyte.MaxValue) ThrowSByteOverflowException();
            return (sbyte)value;
        }
        public static sbyte ToSByte(uint value)
        {
            if (value > (uint)sbyte.MaxValue) ThrowSByteOverflowException();
            return (sbyte)value;
        }
        public static sbyte ToSByte(ushort value)
        {
            if (value > sbyte.MaxValue) ThrowSByteOverflowException();
            return (sbyte)value;
        }

        public static uint ToUInt32(ushort value) => value;
        public static uint ToUInt32(uint value) => value;
        public static uint ToUInt32(byte value) => value;
        public static uint ToUInt32(char value) => value;
        public static uint ToUInt32(bool value) => value ? (uint)bool.True : (uint)bool.False;
        public static uint ToUInt32(int value)
        {
            if (value < 0) ThrowUInt32OverflowException();
            return (uint)value;
        }
        public static uint ToUInt32(sbyte value)
        {
            if (value < 0) ThrowUInt32OverflowException();
            return (uint)value;
        }

        public static ushort ToUInt16(ushort value) => value;
        public static ushort ToUInt16(char value) => value;
        public static ushort ToUInt16(byte value) => value;
        public static ushort ToUInt16(bool value) => value ? (ushort)bool.True : (ushort)bool.False;
        public static ushort ToUInt16(int value) => ToUInt16((uint)value);
        public static ushort ToUInt16(uint value)
        {
            if (value > ushort.MaxValue) ThrowUInt16OverflowException();
            return (ushort)value;
        }
        public static ushort ToUInt16(sbyte value)
        {
            if (value < 0) ThrowUInt16OverflowException();
            return (ushort)value;
        }


        public static char ToChar(ushort value) => (char)value;
        public static char ToChar(char value) => value;
        public static char ToChar(byte value) => (char)value;
        public static char ToChar(int value) => ToChar((uint)value);
        public static char ToChar(uint value)
        {
            if (value > char.MaxValue) ThrowCharOverflowException();
            return (char)value;
        }
        public static char ToChar(sbyte value)
        {
            if (value < 0) ThrowCharOverflowException();
            return (char)value;
        }


        public static short ToInt16(short value) => value;
        public static short ToInt16(byte value) => value;
        public static short ToInt16(sbyte value) => value;
        public static short ToInt16(bool value) => value ? (short)bool.True : (short)bool.False;
        public static short ToInt16(char value)
        {
            if (value > short.MaxValue) ThrowInt16OverflowException();
            return (short)value;
        }
        public static short ToInt16(int value)
        {
            if (value < short.MinValue || value > short.MaxValue) ThrowInt16OverflowException();
            return (short)value;
        }
        public static short ToInt16(uint value)
        {
            if (value > (uint)short.MaxValue) ThrowInt16OverflowException();
            return (short)value;
        }
        public static short ToInt16(ushort value)
        {
            if (value > short.MaxValue) ThrowInt16OverflowException();
            return (short)value;
        }

        public static int ToInt32(ushort value) => value;
        public static int ToInt32(char value) => value;
        public static int ToInt32(byte value) => value;
        public static int ToInt32(sbyte value) => value;
        public static int ToInt32(int value) => value;
        public static int ToInt32(bool value) => value ? bool.True : bool.False;
        public static int ToInt32(uint value)
        {
            if ((int)value < 0) ThrowInt32OverflowException();
            return (int)value;
        }

        private static void ThrowCharOverflowException() { throw new OverflowException("Char"); }
        private static void ThrowByteOverflowException() { throw new OverflowException("Byte"); }
        private static void ThrowSByteOverflowException() { throw new OverflowException("SByte"); }
        private static void ThrowInt16OverflowException() { throw new OverflowException("Int16"); }
        private static void ThrowUInt16OverflowException() { throw new OverflowException("UInt16"); }
        private static void ThrowUInt32OverflowException() { throw new OverflowException("UInt32"); }
        private static void ThrowInt32OverflowException() { throw new OverflowException("Int32"); }
    }
}
