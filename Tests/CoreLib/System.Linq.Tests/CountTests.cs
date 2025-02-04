﻿using System.Collections.Generic;

namespace System.Linq.Tests
{
    static class CountTests
    {
        public static void CountMatchesTallyTests()
        {
            var range = new int[] { 1, 2, 3, 4, 5 };

            CountMatchesTally<int>(5, range);
            CountMatchesTally<int>(5, range.ToList<int>());
        }

        private static void CountMatchesTally<T>(int count, IEnumerable<T> enumerable)
        {
            Assert.AreEqual(count, enumerable.Count());
        }
    }
}
