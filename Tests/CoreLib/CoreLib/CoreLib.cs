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
            StringTests.EqualsTests();

            UnsafeTests.SizeOfTests();
            UnsafeTests.RefAs();

            UnsafeTests.InitBlockTests();
            UnsafeTests.CopyBlockTests();

            AllocTests.AllocEETypeTests();
            AllocTests.AllocSizeTests();

            ArrayTests.ForEachArrayEnumerationTests();

            TypeCastTests.ClassTypeCastTests();

            InterpolatedStringHandlerTests.ToStringAndClear_Clears();
            InterpolatedStringHandlerTests.AppendLiteral();
            InterpolatedStringHandlerTests.AppendFormatted();

            return 0;
        }
    }
}