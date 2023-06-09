using System;

namespace CoreLib
{
    public class StringTests
    {
        public static unsafe void NewStringTests()
        {
            var chars = new char[25];
            string newString = new String(chars);
            Assert.AreEquals(25, newString.Length);
        }
    }
}
