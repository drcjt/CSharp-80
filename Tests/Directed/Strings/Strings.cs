using System;

namespace Strings
{
    public static class Tests
    {
        public static int Main()
        {
            var testString = "Testing 1,2,3";

            if (testString.Length != 13) return 1;

            var fifthCharacter = testString[4];
            if (fifthCharacter != 'i') return 1;

            return 0;
        }
    }
}