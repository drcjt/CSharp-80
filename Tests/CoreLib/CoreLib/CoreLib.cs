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
            TypeCastTests.InterfaceTypeCastTests();

            InterpolatedStringHandlerTests.ToStringAndClear_Clears();
            InterpolatedStringHandlerTests.AppendLiteral();
            InterpolatedStringHandlerTests.AppendFormatted();

            EnumerableTests.ArrayEnumerator_EnumeratesArrayElements();
            EnumerableTests.FibonacciEnumerable_FirstFifteenNumbers_AreCorrect();
            EnumerableTests.GenericArrayEnumerator_EnumeratesArrayElements();

            return 0;
        }
    }
}