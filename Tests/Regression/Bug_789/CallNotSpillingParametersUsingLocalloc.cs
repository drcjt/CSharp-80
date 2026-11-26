using System;
using System.Reflection;
using Xunit;

[assembly: AssemblyVersion("1.0.0.0")]
namespace Regression
{
    public static class CallNotSpillingParametersUsingLocalloc
    {
        [Fact]
        public static void Bug789Test()
        {
            Span<byte> b = stackalloc byte[100];
            Assert.Equal(100, b.Length);
        }
    }
}
