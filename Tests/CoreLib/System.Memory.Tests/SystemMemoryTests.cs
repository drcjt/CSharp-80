namespace System.Memory.Tests
{
    public static class SystemMemoryTests
    {
        public static int Main()
        {
            SpanTests.CtorArray_Integers();
            SpanTests.CtorArray_Objects();

            SpanTests.CtorArrayIntInt1();
            SpanTests.CtorArrayIntIntRangeExtendsToEndOfArray();

            SpanTests.CtorPointerInt();

            ReadOnlySpanTests.CtorArray_Integers();
            ReadOnlySpanTests.CtorArray_Objects();

            ReadOnlySpanTests.CtorArrayIntInt1();
            ReadOnlySpanTests.CtorArrayIntIntRangeExtendsToEndOfArray();

            ReadOnlySpanTests.CtorPointerInt();

            return 0;
        }
    }
}