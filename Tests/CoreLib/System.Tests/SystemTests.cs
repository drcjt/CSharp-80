namespace System.Tests
{
    public static class SystemTests
    {
        public static int Main()
        {
            ArrayTests.GetEnumerator();
            ArrayTests.GetValue_RankOneInt_SetValue();
            ArrayTests.IndexOf_ArrayTests();
            ArrayTests.EmptyTests();

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

            StringTests.Ctor_CharSpan_EmptyString_Tests();
            StringTests.Ctor_CharSpan_Empty();
            StringTests.Ctor_CharSpan_Tests();
            StringTests.Contains_Char_Tests();
            StringTests.EqualsTests();
            StringTests.Contains_Match_Char();
            StringTests.Contains_ZeroLength_Char();
            StringTests.Contains_MultipleMatches_Char();
            StringTests.ImplicitCast_ResultingSpanMatches_Tests();
            StringTests.ImplicitCast_NullString_ReturnsDefaultSpan();

            DateTimeTests.Ctor_Int_Int_Int_Int();
            DateTimeTests.Equals_Tests();

            return 0;
        }
    }
}