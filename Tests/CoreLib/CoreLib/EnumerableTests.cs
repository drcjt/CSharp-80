using System.Collections;
using System.Collections.Generic;

namespace CoreLib
{
    public class GenericArrayEnumerable<T> : IEnumerable<T>
    {
        private T[] _items;
        public GenericArrayEnumerable(T[] items) 
        { 
            _items = items;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var index = 0; index < _items.Length; index++)
            {
                yield return _items[index];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

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

    public static class EnumerableTests
    {
        public static void GenericArrayEnumerator_EnumeratesArrayElements()
        {
            var testArray = new int[] { 1, 2, 3 };
            var enumerable = new GenericArrayEnumerable<int>(testArray);

            int index = 0;
            foreach (var item in enumerable)
            {
                Assert.AreEqual(testArray[index++], item);
            }
        }

        public static void ArrayEnumerator_EnumeratesArrayElements()
        {
            var testArray = new int[] { 1, 2, 3 };
            var enumerable = new ArrayEnumerable(testArray);

            int index = 0;
            foreach (var item in enumerable)
            {
                Assert.AreEqual(testArray[index++], item);
            }
        }

        public static void FibonacciEnumerable_FirstFifteenNumbers_AreCorrect()
        {
            var sequence = new FibonacciEnumerable().GetEnumerator();

            Assert.IsTrue(sequence.MoveNext());
            Assert.AreEqual(0, sequence.Current);

            Assert.IsTrue(sequence.MoveNext());
            Assert.AreEqual(1, sequence.Current);

            Assert.IsTrue(sequence.MoveNext());
            Assert.AreEqual(1, sequence.Current);

            Assert.IsTrue(sequence.MoveNext());
            Assert.AreEqual(2, sequence.Current);

            Assert.IsTrue(sequence.MoveNext());
            Assert.AreEqual(3, sequence.Current);

            Assert.IsTrue(sequence.MoveNext());
            Assert.AreEqual(5, sequence.Current);

            Assert.IsTrue(sequence.MoveNext());
            Assert.AreEqual(8, sequence.Current);

            Assert.IsTrue(sequence.MoveNext());
            Assert.AreEqual(13, sequence.Current);
        }
    }
}
