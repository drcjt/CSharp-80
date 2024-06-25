namespace GenericTypeParameters
{
    internal static class TestRunner
    {
        public static int Main()
        {
            int result = DefaultClass.RunTests(); if (result != 0) return result;
            result = DefaultStruct.RunTests(); if (result != 0) return result;

            return result;
        }
    }
}