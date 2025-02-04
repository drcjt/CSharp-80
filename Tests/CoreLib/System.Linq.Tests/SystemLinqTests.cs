namespace System.Linq.Tests
{
    public static class SystemLinqTests
    {
        public static int Main()
        {
            AnyTests.Any_Tests();

            CountTests.CountMatchesTallyTests();

            ToListTests.ToList_AlwaysCreatesACopy();

            ToArrayTests.ToArray_CreatesACopyWhenNotEmpty();

            return 0;
        }
    }
}