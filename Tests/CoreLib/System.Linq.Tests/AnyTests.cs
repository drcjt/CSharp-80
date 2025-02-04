using System.Collections.Generic;

namespace System.Linq.Tests
{
    static class AnyTests
    {
        public static void Any_Tests()
        {
            Any_Test(new int[0], false);
            Any_Test(new int[1], true);
            Any_Test(new int[2], true);
        }

        private static void Any_Test(IEnumerable<int> source, bool expected)
        {
            Assert.AreEqual(expected, source.Any());
        }
    }
}
