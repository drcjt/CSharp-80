using System.Reflection;
using Xunit;

[assembly: AssemblyVersion("1.0.0.0")]
namespace Regression
{
    public struct S
    {
        public int Value;
    }

    public static class InitObjNotSpilling
    {
        [Fact]
        public static void BugXXXTest()
        {
            Assert.True(N(new S() { Value = 25 }, default(S)));
        }

        public static bool N(S x, S y) => x.Value != y.Value;
    }
}
