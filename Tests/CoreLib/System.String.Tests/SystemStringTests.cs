namespace System.String.Tests
{
    public static class SystemTests
    {
        public static int Main()
        {
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

            return 0;
        }
    }
}