﻿namespace CoreLib
{
    public static class CoreLib
    {
        public static int Main()
        {
            var result = true;
            
            result = result && CharTests.IsBetweenCharTests();
            
            result = result && CharTests.IsAsciiDigit_WithAsciiDigits_ReturnsTrue();
            result = result && CharTests.IsAsciiDigit_WithNonAsciiDigits_ReturnsFalse();

            result = result && Int32Tests.Parse_Valid();

            result = result && StringTests.NewStringTests();

            return result ? 0 : 1;
        }
    }
}