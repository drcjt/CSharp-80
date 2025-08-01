using System;
using Xunit;

namespace CoreLib
{
    public static class AllocTests
    {
        private class TestClass
        {
            private int field1;
            private bool field2;
            private bool field3;
        }

        [Fact]
        public static void AllocStringSizeTests()
        {
            AllocStringSizeTest(new char[5]);
        }

        private static void AllocStringSizeTest(char[] chars)
        {
            // This is not GC safe
            var totalMemory = GC.GetTotalMemory();

            var str1 = new String(chars);

            // intptr   EEType      2
            // ushort   length      2
            // char     elements    5 x 2
            // -----------------------
            // Total                14

            Assert.Equal(14, GC.GetTotalMemory() - totalMemory);
        }

        [Fact]
        public static void AllocObjectSizeTest()
        {
            // This is not GC safe
            var totalMemory = GC.GetTotalMemory();

            var obj = new TestClass();

            // intptr   EEType      2
            // int      field1      4
            // bool     field2      1
            // bool     field3      1 + 2 padding for int alignment
            // -----------------------
            //                      10

            Assert.Equal(10, GC.GetTotalMemory() - totalMemory);
        }

        [Fact]
        public static void AllocArraySizeTest()
        {
            // This is not GC safe
            var totalMemory = GC.GetTotalMemory();

            var chars = new char[5];

            // intptr   EEType      2
            // ushort   length      2
            // char     elements    5 x 2
            // -----------------------
            // Total                14

            Assert.Equal(14, GC.GetTotalMemory() - totalMemory);
        }

        [Fact]
        static public void AllocEETypeTests()
        {
            var str1 = new String(new char[5]);
            var str2 = new String(new char[3]);

            // Check EEType is same for both string objects
            Assert.Equal(str1.GetEEType(), str2.GetEEType());

            var class1 = new TestClass();
            var class2 = new TestClass();

            // Check EEType is same for both instances of TestClass
            Assert.Equal(class1.GetEEType(), class2.GetEEType());

            // Check EEType is different for String and TestClass instances
            Assert.NotEqual(str1.GetEEType(), class1.GetEEType());
        }
    }
}
