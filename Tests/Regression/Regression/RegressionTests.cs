namespace Regression
{
    public static class Regression
    {
        public static int Main()
        {
            Bug87();

            nuint testValue = 123;
            Assert.Equals(testValue, MethodCall_WithNuintParameter_CompilesWithoutErrors(testValue));

            return 0;
        }

        public static void Bug87()
        {
            char[] test = new char[1] { 'a' };
            Bug87_Method(test[0]);
        }

        private static char Bug87_Method(char ch)
        {
            return ch;
        }

        private static nuint MethodCall_WithNuintParameter_CompilesWithoutErrors(nuint n)
        {
            return n;
        }
    }
}