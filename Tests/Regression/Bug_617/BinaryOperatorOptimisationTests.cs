using Internal.Runtime.CompilerServices;
using Xunit;

namespace Regression
{
    public static class BinaryOperatorOptimizationTests
    {
        [Fact]
        public unsafe static void Bug617()
        {
            byte value = 42;
            Unsafe.Add<byte>(ref value, 0);
        }
    }
}
