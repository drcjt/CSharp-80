using System.Collections;

namespace CoreLib
{
    public class ArrayEnumerable(int[] array) : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            var index = 0;
            while (index < array.Length)
            {
                yield return array[index++];
            }
        }
    }

    public class FibonacciEnumerable : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            yield return 0;
            yield return 1;

            int previous = 0;
            int next = 1;
            while (true)
            {
                int sum = previous + next;
                previous = next;
                next = sum;
                yield return sum; 
            }
        }
    }

    public class EnumerableTests
    {
        public static void ArrayEnumerator_EnumeratesArrayElements()
        {
            var testArray = new int[] { 1, 2, 3 };
            var enumerable = new ArrayEnumerable(testArray);

            int index = 0;
            foreach (var item in enumerable)
            {
                Assert.AreEquals(testArray[index++], item);
            }
        }

        public static void FibonacciEnumerable_FirstFifteenNumbers_AreCorrect()
        {
            var sequence = new FibonacciEnumerable().GetEnumerator();

            Assert.IsTrue(sequence.MoveNext());
            Assert.AreEquals(0, sequence.Current);

            Assert.IsTrue(sequence.MoveNext());
            Assert.AreEquals(1, sequence.Current);

            Assert.IsTrue(sequence.MoveNext());
            Assert.AreEquals(1, sequence.Current);

            Assert.IsTrue(sequence.MoveNext());
            Assert.AreEquals(2, sequence.Current);

            Assert.IsTrue(sequence.MoveNext());
            Assert.AreEquals(3, sequence.Current);

            Assert.IsTrue(sequence.MoveNext());
            Assert.AreEquals(5, sequence.Current);

            Assert.IsTrue(sequence.MoveNext());
            Assert.AreEquals(8, sequence.Current);

            Assert.IsTrue(sequence.MoveNext());
            Assert.AreEquals(13, sequence.Current);
        }
    }
}
