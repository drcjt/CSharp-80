namespace System.Tests
{
    public static class SystemTests
    {
        public static int Main()
        {
            ArrayTests.GetEnumerator();
            ArrayTests.GetValue_RankOneInt_SetValue();
            ArrayTests.IndexOf_ArrayTests();

            ObjectTests.EqualsTests();
            ObjectTests.ReferenceEqualsTests();

            ByteTests.Ctor_Empty();
            ByteTests.MinValue();
            ByteTests.MaxValue();
            ByteTests.EqualsTests();

            SByteTests.Ctor_Empty();
            SByteTests.MinValue();
            SByteTests.MaxValue();
            SByteTests.EqualsTests();

            CharTests.EqualsTests();
            CharTests.IsAsciiDigit_WithAsciiDigits_ReturnsTrue();
            CharTests.IsAsciiDigit_WithNonAsciiDigits_ReturnsFalse();
            CharTests.IsBetweenCharTests();

            Int16Tests.Ctor_Empty();
            Int16Tests.MinValue();
            Int16Tests.MaxValue();
            Int16Tests.EqualsTests();

            UInt16Tests.Ctor_Empty();
            UInt16Tests.MinValue();
            UInt16Tests.MaxValue();
            UInt16Tests.EqualsTests();

            Int32Tests.Ctor_Empty();
            Int32Tests.MinValue();
            Int32Tests.MaxValue();
            Int32Tests.ToStringTests();
            Int32Tests.Parse_Valid();
            Int32Tests.EqualsTests();

            UInt32Tests.Ctor_Empty();
            UInt32Tests.MinValue();
            UInt32Tests.MaxValue();
            UInt32Tests.EqualsTests();

            IntPtrTests.EqualsTests();

            UIntPtrTests.EqualsTests();

            return 0;
        }
    }
}