using Xunit;

namespace System.Memory.Tests
{
    public static partial class ReadOnlySpanTests
    {
        [Fact]
        public static void CtorArray_Integers()
        {
            int[] a = { 91, 92, -93, 94 };
            ReadOnlySpan<int> span;

            span = new ReadOnlySpan<int>(a);
            span.Validate(91, 92, -93, 94);

            span = new ReadOnlySpan<int>(a, 0, a.Length);
            span.Validate(91, 92, -93, 94);
        }

        [Fact]
        public static void CtorArray_Objects()
        {
            object o1 = new object();
            object o2 = new object();
            object[] a = { o1, o2 };
            ReadOnlySpan<object> span;

            span = new ReadOnlySpan<object>(a);
            span.ValidateReferenceType(o1, o2);

            span = new ReadOnlySpan<object>(a, 0, a.Length);
            span.ValidateReferenceType(o1, o2);
        }
    }
}
