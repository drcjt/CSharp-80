using System.Runtime;

namespace CoreLib
{
    public class StringTests
    {
        public static void NewStringTests()
        {
            string newString = RuntimeImports.NewString(25);
            Assert.Equals(25, newString.Length);
        }
    }
}
