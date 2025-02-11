namespace System.Collections.Tests
{
    public static class SystemCollectionsTests
    {
        public static int Main()
        {
            EqualityComparerTests.Default_ForType_CreatesNoMoreThanOneComparerInstance();
            EqualityComparerTests.EqualsTests();

            return 0;
        }
    }
}