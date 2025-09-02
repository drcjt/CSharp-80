using Xunit;

namespace Regression
{
    internal static class SpillImportStElemTests
    {
        [Fact]
        public static void Test()
        {
            Assert.Equal(37, StElemShouldSpillWhenAppendingStatement());
        }

        public static int StElemShouldSpillWhenAppendingStatement()
        {
            object?[] arr = new object[1];
            arr[0] = 37;

            int size = 1;

            object value = arr[--size]!;
            arr[size] = null;

            return (int)value;
        }
    }
}
