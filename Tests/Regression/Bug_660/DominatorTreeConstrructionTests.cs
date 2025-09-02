using Xunit;

namespace Regression
{
    public static class DominatorTreeConstructionTests
    {
        [Fact]
        public static void Bug660Test()
        {
            int[] balls = new int[10];

            bool bouncing = true;
            while (bouncing)
            {
                for (int i = 0; i < balls.Length; i++)
                {
                    if (i == 5)
                    {
                        bouncing = false;
                        break;
                    }
                }
            }
        }
    }
}
