﻿using Xunit;

namespace System.Memory.Tests
{
    public static partial class SpanTests
    {
        [Fact]
        public static void CtorArrayIntInt1()
        {
            int[] a = { 90, 91, 92, 93, 94, 95, 96, 97, 98 };
            Span<int> span = new Span<int>(a, 3, 2);
            span.Validate(93, 94);
        }

        [Fact]
        public static void CtorArrayIntIntRangeExtendsToEndOfArray()
        {
            int[] a = { 90, 91, 92, 93, 94, 95, 96, 97, 98 };
            Span<int> span = new Span<int>(a, 4, 5);
            span.Validate(94, 95, 96, 97, 98);
        }

    }
}
