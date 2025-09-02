using Xunit;

namespace Regression
{
    public static class VarTypeMappingTests
    {
        [Fact]
        public static void Bug87()
        {
            char[] test = new char[1] { 'a' };
            Bug87_Method(test[0]);
        }

        private static char Bug87_Method(char ch)
        {
            return ch;
        }

        [Fact]
        public static void MethodCall_WithNuintParameter_CompilesWithoutErrors()
        {
            nuint testValue = 123;
            Assert.Equal(testValue, MethodCall_WithNuintParameter_CompilesWithoutErrors(testValue));
        }

        private static nuint MethodCall_WithNuintParameter_CompilesWithoutErrors(nuint n)
        {
            return n;
        }
    }
}
