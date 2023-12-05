namespace CoreLib
{
    public class ArrayTests
    {
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

            Assert.AreEquals(55, sum);
        }
    }
}
