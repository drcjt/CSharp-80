using System.Collections.Generic;

namespace System.Linq.Tests
{
    static class ToArrayTests
    {
        public static void ToArray_CreatesACopyWhenNotEmpty()
        {
            int[] sourceArray = new int[] { 1, 2, 3, 4, 5 };
            int[] resultArray = sourceArray.ToArray();

            Assert.IsTrue(sourceArray != resultArray);
            Assert.AreEnumerablesEqual(sourceArray, resultArray);
        }
    }
}
