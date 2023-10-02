using System;

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

            Assert.AreEquals(14, GC.GetTotalMemory() - totalMemory);
        }

        private static void AllocObjectSizeTest()
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

            Assert.AreEquals(10, GC.GetTotalMemory() - totalMemory);
        }

        private static void AllocArraySizeTest()
        {
            // This is not GC safe
            var totalMemory = GC.GetTotalMemory();

            var chars = new char[5];

            // intptr   EEType      2
            // ushort   length      2
            // char     elements    5 x 2
            // -----------------------
            // Total                14

            Assert.AreEquals(14, GC.GetTotalMemory() - totalMemory);
        }

        static public void AllocSizeTests()
        {
            AllocArraySizeTest();
            AllocStringSizeTest(new char[5]);
            AllocObjectSizeTest();
        }

        static public void AllocEETypeTests()
        {
            var str1 = new String(new char[5]);
            var str2 = new String(new char[3]);

            // Check EEType is same for both string objects
            Assert.AreEquals(str1.GetEEType(), str2.GetEEType());

            var class1 = new TestClass();
            var class2 = new TestClass();

            // Check EEType is same for both instances of TestClass
            Assert.AreEquals(class1.GetEEType(), class2.GetEEType());

            // Check EEType is different for String and TestClass instances
            Assert.AreNotEquals(str1.GetEEType(), class1.GetEEType());
        }
    }
}
