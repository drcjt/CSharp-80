using System.Reflection;
using Internal.Runtime.CompilerServices;
using Xunit;

[assembly: AssemblyVersion("1.0.0.0")]
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
