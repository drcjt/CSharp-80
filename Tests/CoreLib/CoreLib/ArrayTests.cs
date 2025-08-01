using Xunit;

namespace CoreLib
{
    public static class ArrayTests
    {
        [Fact]
        public static void ForEachArrayEnumerationTests()
        {            
            int sum = 0;
            var array = new int[10];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = i + 1;
            }

            // Test array element enumeration using foreach syntax
            foreach (int i in array)
            {
                sum += i;
            }

            Assert.Equal(55, sum);
        }
    }
}
