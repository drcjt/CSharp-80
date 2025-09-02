using System.Reflection;
using Xunit;

[assembly: AssemblyVersion("1.0.0.0")]
namespace Regression
{
    public static class StackAllocZeroSizeTests
    {
        [Fact]
        public static void Bug206_Test()
        {
            Assert.Equal<nuint>(0, Bug206(0));
        }

        public unsafe static nuint Bug206(int count)
        {
            byte* b = stackalloc byte[count];
            return ((nuint)b);
        }
    }
}
