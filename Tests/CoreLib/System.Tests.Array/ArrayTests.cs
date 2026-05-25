using Xunit;
using System.Collections;

namespace System.Tests
{
    internal static class ArrayTests
    {
        [Fact]
        public static void GetValue_RankOneInt_SetValue()
        {
            var intArray = new int[] { 7, 8, 9 };
            Array array = intArray;

            Assert.Equal(7, array.GetValue(0));
            array.SetValue(41, 0);
            Assert.Equal(41, array.GetValue(0));

            Assert.Equal(8, array.GetValue(1));
            array.SetValue(42, 1);
            Assert.Equal(42, array.GetValue(1));

            Assert.Equal(9, array.GetValue(2));
            array.SetValue(43, 2);
            Assert.Equal(43, array.GetValue(2));
        }

        [Fact]
        public static void IndexOf_ArrayTests()
        {
            var sbyteArray = new sbyte[] { 1, 2, 3, 3 };
            IndexOf_SZArray<sbyte>(sbyteArray, (sbyte)1, 0);            
            IndexOf_SZArray<sbyte>(sbyteArray, (sbyte)3, 2);
            IndexOf_SZArray<sbyte>(sbyteArray, (sbyte)2, 1);
            IndexOf_SZArray<sbyte>(Array.Empty<sbyte>(), (sbyte)1, -1);

            var byteArray = new byte[] { 1, 2, 3, 3 };
            IndexOf_SZArray<byte>(byteArray, (byte)1, 0);
            IndexOf_SZArray<byte>(byteArray, (byte)3, 2);
            IndexOf_SZArray<byte>(byteArray, (byte)2, 1);
            IndexOf_SZArray<byte>(Array.Empty<byte>(), (byte)1, -1);

            var shortArray = new short[] { 1, 2, 3, 3 };
            IndexOf_SZArray<short>(shortArray, (short)1, 0);
            IndexOf_SZArray<short>(shortArray, (short)3, 2);
            IndexOf_SZArray<short>(shortArray, (short)2, 1);
            IndexOf_SZArray<short>(Array.Empty<short>(), (short)1, -1);

            var ushortArray = new ushort[] { 1, 2, 3, 3 };
            IndexOf_SZArray<ushort>(ushortArray, (ushort)1, 0);
            IndexOf_SZArray<ushort>(ushortArray, (ushort)3, 2);
            IndexOf_SZArray<ushort>(ushortArray, (ushort)2, 1);
            IndexOf_SZArray<ushort>(Array.Empty<ushort>(), (ushort)1, -1);

            var intArray = new int[] { 7, 7, 8, 8, 9, 9 };
            IndexOf_SZArray<int>(intArray, 8, 2);
            IndexOf_SZArray<int>(intArray, 9, 4);
            IndexOf_SZArray<int>(intArray, 10, -1);

            var uintArray = new uint[] { 1, 2, 3, 3 };
            IndexOf_SZArray<uint>(uintArray, (uint)1, 0);
            IndexOf_SZArray<uint>(uintArray, (uint)3, 2);
            IndexOf_SZArray<uint>(uintArray, (uint)2, 1);
            IndexOf_SZArray<uint>(new uint[0], (uint)1, -1);

            var charArray = new char[] { (char)1, (char)2, (char)3, (char)3 };
            IndexOf_SZArray<char>(charArray, (char)1, 0);
            IndexOf_SZArray<char>(charArray, (char)3, 2);
            IndexOf_SZArray<char>(charArray, (char)2, 1);
            IndexOf_SZArray<char>(Array.Empty<char>(), (char)1, -1);

            var boolArray = new bool[] { true, false, false, true };
            IndexOf_SZArray<bool>(boolArray, true, 0);
            IndexOf_SZArray<bool>(boolArray, false, 1);
            IndexOf_SZArray<bool>(Array.Empty<bool>(), true, -1);

            var intPtrArray = new IntPtr[] { (IntPtr)1, (IntPtr)2, (IntPtr)3, (IntPtr)3 };
            IndexOf_SZArray<IntPtr>(intPtrArray, (IntPtr)1, 0);
            IndexOf_SZArray<IntPtr>(intPtrArray, (IntPtr)3, 2);
            IndexOf_SZArray<IntPtr>(intPtrArray, (IntPtr)2, 1);
            IndexOf_SZArray<IntPtr>(Array.Empty<IntPtr>(), (IntPtr)1, -1);

            var uintPtrArray = new UIntPtr[] { (UIntPtr)1, (UIntPtr)2, (UIntPtr)3, (UIntPtr)3 };
            IndexOf_SZArray<UIntPtr>(uintPtrArray, (UIntPtr)1, 0);
            IndexOf_SZArray<UIntPtr>(uintPtrArray, (UIntPtr)3, 2);
            IndexOf_SZArray<UIntPtr>(uintPtrArray, (UIntPtr)2, 1);
            IndexOf_SZArray<UIntPtr>(Array.Empty<UIntPtr>(), (UIntPtr)1, -1);
        }

        private static void IndexOf_SZArray<T>(T[] array, T value, int expected)
        {
            Assert.Equal(expected, Array.IndexOf<T>(array, value, 0, array.Length));

            IndexOf_Array(array, value, expected);
        }

        private static void IndexOf_Array(Array array, object? value, int expected)
        {
            IList iList = array;

            Assert.Equal(expected, iList.IndexOf(value));
            Assert.Equal(expected >= 0, iList.Contains(value));
        }

        [Fact]
        public static void GetEnumerator()
        {
            GetEnumerator(Array.Empty<int>());
            GetEnumerator(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            GetEnumerator(new char[] { 'a', 'b', 'c' });
        }

        public static void GetEnumerator(Array array)
        {
            var enumerator1 = array.GetEnumerator();
            var enumerator2 = array.GetEnumerator();
            Assert.True(enumerator1 != enumerator2);

            IEnumerator enumerator = array.GetEnumerator();
            for (int i = 0; i < 2; i++)
            {
                int counter = 0;
                while (enumerator.MoveNext())
                {
                    Assert.Equal(array[counter], enumerator.Current);
                    counter++;
                }

                Assert.False(enumerator.MoveNext());
                Assert.Equal(array.Length, counter);

                enumerator.Reset();
            }
        }

        [Fact]
        public static void EmptyTests()
        {
            Assert.True(Array.Empty<int>() != null);
            Assert.Equal(0, Array.Empty<int>().Length);
            Assert.Same(Array.Empty<int>(), Array.Empty<int>());

            Assert.True(Array.Empty<object>() != null);
            Assert.Equal(0, Array.Empty<object>().Length);
            Assert.Same(Array.Empty<object>(), Array.Empty<object>());
        }
    }
}
