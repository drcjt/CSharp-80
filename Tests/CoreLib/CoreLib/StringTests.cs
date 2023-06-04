using System;
using System.Runtime;

namespace CoreLib
{
    public class StringTests
    {
        public static void NewStringTests()
        {
            string newString = RuntimeImports.NewString(EETypePtr.EETypePtrOf<String>(), 25);
            Assert.Equals(25, newString.Length);
        }
    }
}
