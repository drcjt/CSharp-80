using System.Runtime;

namespace CoreLib
{
    public class StringTests
    {
        public static bool NewStringTests()
        {
            var result = true;

            string newString = RuntimeImports.NewString(25);
            result = result && newString.Length == 25;

            return result;
        }
    }
}
