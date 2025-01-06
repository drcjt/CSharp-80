namespace System.Tests
{
    public static class SystemTests
    {
        public static int Main()
        {
            ArrayTests.GetValue_RankOneInt_SetValue();
            ArrayTests.GetEnumerator();

            ObjectTests.EqualsTests();

            return 0;
        }
    }
}