using System.Collections.Generic;
using Xunit;

namespace System.Linq.Tests
{
    static class CountTests
    {
        [Fact]
        public static void CountMatchesTallyTests()
        {
            var range = new int[] { 1, 2, 3, 4, 5 };

            CountMatchesTally<int>(5, range);
            CountMatchesTally<int>(5, range.ToList<int>());
        }

        private static void CountMatchesTally<T>(int count, IEnumerable<T> enumerable)
        {
            Assert.Equal(count, enumerable.Count());
        }
    }
}
