namespace System.Tests
{
    public static class SystemTests
    {
        public static int Main()
        {
            ArrayTests.GetValue_RankOneInt_SetValue();
            ArrayTests.GetEnumerator();

            ObjectTests.EqualsTests();
            ObjectTests.ReferenceEqualsTests();

            CharTests.EqualsTests();
            CharTests.IsAsciiDigit_WithAsciiDigits_ReturnsTrue();
            CharTests.IsAsciiDigit_WithNonAsciiDigits_ReturnsFalse();
            CharTests.IsBetweenCharTests();

            Int32Tests.Ctor_Empty();
            Int32Tests.MinValue();
            Int32Tests.MaxValue();
            Int32Tests.ToStringTests();
            Int32Tests.Parse_Valid();
            Int32Tests.EqualsTests();

            IntPtrTests.EqualsTests();

            return 0;
        }
    }
}