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

        // Very unsafe method to get EEType of a managed object
        // This is not GC safe
        static unsafe ushort GetEEType(object obj)
        {
            var objectptr = *(void**)Internal.Runtime.CompilerServices.Unsafe.AsPointer(ref obj);
            var eetypeptr = *((ushort*)objectptr);

            return eetypeptr;
        }

        static public void AllocSizeTests()
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

            totalMemory = GC.GetTotalMemory();
            
            var str1 = new String(chars);

            // intptr   EEType      2
            // ushort   length      2
            // char     elements    5 x 2
            // -----------------------
            // Total                14

            Assert.AreEquals(14, GC.GetTotalMemory() - totalMemory);

            totalMemory = GC.GetTotalMemory();

            var obj = new TestClass();

            // intptr   EEType      2
            // int      field1      4
            // bool     field2      1
            // bool     field3      1 + 2 padding for int alignment
            // -----------------------
            //                      10

            Assert.AreEquals(10, GC.GetTotalMemory() - totalMemory);
        }

        static public void AllocEETypeTests()
        {
            var str1 = new String(new char[5]);
            var str2 = new String(new char[3]);

            // Check EEType is same for both string objects
            Assert.AreEquals(GetEEType(str1), GetEEType(str2));

            var class1 = new TestClass();
            var class2 = new TestClass();

            // Check EEType is same for both instances of TestClass
            Assert.AreEquals(GetEEType(class1), GetEEType(class2));

            // Check EEType is different for String and TestClass instances
            Assert.AreNotEquals(GetEEType(str1), GetEEType(class1));
        }
    }
}
