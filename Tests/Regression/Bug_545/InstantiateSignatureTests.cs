using System.Reflection;
using Xunit;

[assembly: AssemblyVersion("1.0.0.0")]
namespace Regression
{
    public static class InstantiateSignatureTests
    {
        [Fact]
        public static void Bug545()
        {
            Assert.Equal(10, Bug545Method<int>().Length);
        }

        public static T[] Bug545Method<T>()
        {
            var t = new Bug545Test<T>();
            return t.ToArray();
        }
    }

    public class Bug545Test<T>()
    {
        public T[] ToArray()
        {
            var array = new T[10];
            return array;
        }
    }
}
