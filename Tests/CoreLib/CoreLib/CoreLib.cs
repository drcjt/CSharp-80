namespace CoreLib
{
    public static class CoreLib
    {
        public static int Main()
        {
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

            InterpolatedStringHandlerTests.SingleInterpolation();
            InterpolatedStringHandlerTests.DoubleInterpolation();
            InterpolatedStringHandlerTests.TripleInterpolation();
            InterpolatedStringHandlerTests.QuadInterpolation();

            InterpolatedStringHandlerTests.ToStringAndClear_Clears();
            InterpolatedStringHandlerTests.AppendLiteral();
            InterpolatedStringHandlerTests.AppendFormatted();

            EnumerableTests.ArrayEnumerator_EnumeratesArrayElements();
            EnumerableTests.FibonacciEnumerable_FirstFifteenNumbers_AreCorrect();
            EnumerableTests.GenericArrayEnumerator_EnumeratesArrayElements();

            ArrayListTests.RunArrayListTests();

            return 0;
        }
    }
}