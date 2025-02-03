using System.Collections.Generic;

namespace System.Linq.Tests
{
    static class ToListTests
    {
        public static void ToList_AlwaysCreatesACopy()
        {
            List<int> sourceList = [1, 2, 3, 4, 5];
            List<int> resultList = sourceList.ToList();

            Assert.IsTrue(sourceList != resultList);
            Assert.AreEnumerablesEqual(sourceList, resultList);
        }
    }
}
