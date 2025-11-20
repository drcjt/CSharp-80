using System.Collections.Generic;
using Xunit;

namespace System.Linq.Tests
{
    static class RangeTests
    {
        static IEnumerable<int> GetRange(int start, int count) => Enumerable.Range(start, count);

        [Fact]
        public static void Range_ProduceCorrectSequence()
        {
            IEnumerable<int> rangeSequence = GetRange(1, 100);
            int expected = 0;
            foreach (int val in rangeSequence)
            {
                expected++;
                Assert.Equal(expected, val);
            }

            Assert.Equal(100, expected);
        }

        [Fact]
        public static void Range_ToList_ProduceCorrectResult()
        {
            var list = GetRange(1, 100).ToList();
            Assert.Equal(100, list.Count);
            for (int i = 0; i < list.Count; i++)
                Assert.Equal(i + 1, list[i]);
        }

        [Fact]
        public static void Range_ZeroCountLeadToEmptySequence()
        {
            int[]? array = GetRange(1, 0).ToArray();
            int[]? array2 = GetRange(int.MinValue, 0).ToArray();
            int[]? array3 = GetRange(int.MaxValue, 0).ToArray();
            Assert.Equal(0, array.Length);
            Assert.Equal(0, array2.Length);
            Assert.Equal(0, array3.Length);
        }
    }
}
