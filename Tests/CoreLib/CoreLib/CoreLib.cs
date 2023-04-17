﻿namespace CoreLib
{
    public static class CoreLib
    {
        public static int Main()
        {   
            CharTests.IsBetweenCharTests();
            
            CharTests.IsAsciiDigit_WithAsciiDigits_ReturnsTrue();
            CharTests.IsAsciiDigit_WithNonAsciiDigits_ReturnsFalse();

            Int32Tests.Parse_Valid();

            StringTests.NewStringTests();

            UnsafeTests.SizeOfTests();
            UnsafeTests.RefAs();

            UnsafeTests.InitBlockTests();

            return 0;
        }
    }
}