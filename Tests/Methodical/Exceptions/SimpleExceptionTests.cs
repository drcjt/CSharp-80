namespace Exceptions
{
    internal static class SimpleExceptionTests
    {
        private static int Try_NoThrow()
        {
            int result;

            try
            {
                result = 0;
            }
            catch
            {
                result = 1;
            }

            return result;
        }

        public static int RunTests()
        {
            return Try_NoThrow();
        }
    }
}
