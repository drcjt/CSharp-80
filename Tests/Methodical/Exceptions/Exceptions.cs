namespace Exceptions
{
    public static class Test
    {
        public static int Main()
        {
            int result = 0;
            result = SimpleExceptionTests.RunTests(); if (result != 0) return result;
     
            return 0;
        }
    }
}