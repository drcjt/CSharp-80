using System.Collections.Generic;
using Xunit;

namespace System.Tests
{
    public static class MathTests
    {
        public static IEnumerable<object[]> Clamp_SignedInt_TestData()
        {
            yield return new object[] { -1, -1, 1, -1 };
            yield return new object[] { 0, -1, 1, 0 };
            yield return new object[] { 1, -1, 1, 1 };
            yield return new object[] { 1, -1, 1, 1 };

            yield return new object[] { -2, -1, 1, -1 };
            yield return new object[] { 2, -1, 1, 1 };
        }

        public static IEnumerable<object[]> Clamp_UnsignedInt_TestData()
        {
            yield return new object[] { 1, 1, 3, 1 };
            yield return new object[] { 2, 1, 3, 2 };
            yield return new object[] { 3, 1, 3, 3 };
            yield return new object[] { 1, 1, 1, 1 };

            yield return new object[] { 0, 1, 3, 1 };
            yield return new object[] { 4, 1, 3, 3 };
        }

        [Theory]
        [MemberData(nameof(Clamp_SignedInt_TestData))]
        public static void Clamp_SByte(sbyte value, sbyte min, sbyte max, sbyte expected)
        {
            Assert.Equal(expected, Math.Clamp(value, min, max));
        }

        [Theory]
        [MemberData(nameof(Clamp_UnsignedInt_TestData))]
        public static void Clamp_Byte(byte value, byte min, byte max, byte expected)
        {
            Assert.Equal(expected, Math.Clamp(value, min, max));
        }

        [Theory]
        [MemberData(nameof(Clamp_SignedInt_TestData))]
        public static void Clamp_Short(short value, short min, short max, short expected)
        {
            Assert.Equal(expected, Math.Clamp(value, min, max));
        }

        [Theory]
        [MemberData(nameof(Clamp_UnsignedInt_TestData))]
        public static void Clamp_UShort(ushort value, ushort min, ushort max, ushort expected)
        {
            Assert.Equal(expected, Math.Clamp(value, min, max));
        }

        [Theory]
        [MemberData(nameof(Clamp_SignedInt_TestData))]
        public static void Clamp_Int(int value, int min, int max, int expected)
        {
            Assert.Equal(expected, Math.Clamp(value, min, max));
        }

        [Theory]
        [MemberData(nameof(Clamp_UnsignedInt_TestData))]
        public static void Clamp_UInt(uint value, uint min, uint max, uint expected)
        {
            Assert.Equal(expected, Math.Clamp(value, min, max));
        }

        [Theory]
        [MemberData(nameof(Clamp_SignedInt_TestData))]
        public static void Clamp_NInt(int value, int min, int max, int expected)
        {
            Assert.Equal((nint)expected, Math.Clamp((nint)value, (nint)min, (nint)max));
        }

        [Theory]
        [MemberData(nameof(Clamp_UnsignedInt_TestData))]
        public static void Clamp_NUInt(uint value, uint min, uint max, uint expected)
        {
            Assert.Equal((nuint)expected, Math.Clamp((nuint)value, (nuint)min, (nuint)max));
        }
    }
}
