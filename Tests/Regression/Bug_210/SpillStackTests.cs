using Xunit;

namespace Regression
{
    public static class SpillStackTests
    {
        [Fact]
        public static void Bug210()
        {
            Assert.Equal(1, Bug210_SpillStack());
        }

        private static int Bug210_SpillStack()
        {
            int x = 0;
            int y = 1;
            x += y == 1 ? 1 : 0;

            return x;
        }
    }
}
