using System.Collections.Generic;
using Xunit;

namespace System.Linq.Tests
{
    static class ToArrayTests
    {
        public static void ToArray_CreatesACopyWhenNotEmpty()
        {
            int[] sourceArray = new int[] { 1, 2, 3, 4, 5 };
            int[] resultArray = sourceArray.ToArray();

            Assert.True(sourceArray != resultArray);
            Assert.EqualEnumerable(sourceArray, resultArray);
        }
    }
}
