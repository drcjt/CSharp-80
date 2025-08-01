﻿using System.Collections.Generic;
using Xunit;

namespace System.Linq.Tests
{
    static class ToArrayTests
    {
        [Fact]
        public static void ToArray_CreatesACopyWhenNotEmpty()
        {
            int[] sourceArray = new int[] { 1, 2, 3, 4, 5 };
            int[] resultArray = sourceArray.ToArray();

            Assert.True(sourceArray != resultArray);
            Assert.Equal(sourceArray, resultArray);
        }
    }
}
