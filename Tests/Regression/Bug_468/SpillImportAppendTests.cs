using Xunit;

namespace Regression
{
    internal class SpillImportAppendTests
    {
        [Fact]
        public static void Test()
        {
            Assert.Equal(2, new SpillImportAppendTests().SpillOnStFldImport());
        }

        private int x = 2;
        public int SpillOnStFldImport()
        {
            var temp = x;
            x = 3;
            return temp;
        }
    }
}
