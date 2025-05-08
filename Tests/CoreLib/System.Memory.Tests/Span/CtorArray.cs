namespace System.Memory.Tests
{
    public static partial class SpanTests
    {
        public static void CtorArray_Integers()
        {
            int[] a = { 91, 92, -93, 94 };
            Span<int> span;

            span = new Span<int>(a);
            span.Validate(91, 92, -93, 94);

            span = new Span<int>(a, 0, a.Length);
            span.Validate(91, 92, -93, 94);
        }

        public static void CtorArray_Objects()
        {
            object o1 = new object();
            object o2 = new object();
            object[] a = { o1, o2 };
            Span<object> span;

            span = new Span<object>(a);
            span.ValidateReferenceType(o1, o2);

            span = new Span<object>(a, 0, a.Length);
            span.ValidateReferenceType(o1, o2);
        }
    }
}
