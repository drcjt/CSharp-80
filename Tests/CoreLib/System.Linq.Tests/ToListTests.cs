using System.Collections.Generic;
using Xunit;

namespace System.Linq.Tests
{
    static class ToListTests
    {
        public static void ToList_AlwaysCreatesACopy()
        {
            List<int> sourceList = [1, 2, 3, 4, 5];
            List<int> resultList = sourceList.ToList();

            Assert.True(sourceList != resultList);
            Assert.Equal(sourceList, resultList);
        }
    }
}
