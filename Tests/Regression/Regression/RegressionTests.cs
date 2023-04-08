namespace Regression
{
    public static class Regression
    {
        public static int Main()
        {
            Bug87();

            nuint testValue = 123;
            Assert.Equals(testValue, MethodCall_WithNuintParameter_CompilesWithoutErrors(testValue));

            Assert.Equals(1, Bug210_SpillStack());

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

        private static int Bug210_SpillStack()
        {
            int x = 0;
            int y = 1;
            x += y == 1 ? 1 : 0;

            return x;
        }

        private static nuint MethodCall_WithNuintParameter_CompilesWithoutErrors(nuint n)
        {
            return n;
        }
    }
}