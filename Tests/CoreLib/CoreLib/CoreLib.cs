namespace CoreLib
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
            StringTests.SubstringTests();

            UnsafeTests.SizeOfTests();
            UnsafeTests.RefAs();

            UnsafeTests.InitBlockTests();
            UnsafeTests.CopyBlockTests();

            AllocTests.AllocEETypeTests();
            AllocTests.AllocSizeTests();

            ArrayTests.ForEachArrayEnumerationTests();

            TypeCastTests.ClassTypeCastTests();

            return 0;
        }
    }
}