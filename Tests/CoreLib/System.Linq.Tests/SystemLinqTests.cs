namespace System.Linq.Tests
{
    public static class SystemLinqTests
    {
        public static int Main()
        {
            ToListTests.ToList_AlwaysCreatesACopy();

            return 0;
        }
    }
}