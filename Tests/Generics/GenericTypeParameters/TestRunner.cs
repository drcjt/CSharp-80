namespace GenericTypeParameters
{
    internal static class TestRunner
    {
        public static int Main()
        {
            var result = DefaultClass.RunTests();
            result &= DefaultStruct.RunTests();

            return result ? 0 : 1;
        }
    }
}