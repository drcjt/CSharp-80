﻿using System.Collections;
using System.Collections.Generic;
using Xunit;

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
        [Fact]
        public static void GenericArrayEnumerator_EnumeratesArrayElements()
        {
            var testArray = new int[] { 1, 2, 3 };
            var enumerable = new GenericArrayEnumerable<int>(testArray);

            int index = 0;
            foreach (var item in enumerable)
            {
                Assert.Equal(testArray[index++], item);
            }
        }

        [Fact]
        public static void ArrayEnumerator_EnumeratesArrayElements()
        {
            var testArray = new int[] { 1, 2, 3 };
            var enumerable = new ArrayEnumerable(testArray);

            int index = 0;
            foreach (var item in enumerable)
            {
                Assert.Equal(testArray[index++], item);
            }
        }

        [Fact]
        public static void FibonacciEnumerable_FirstFifteenNumbers_AreCorrect()
        {
            var sequence = new FibonacciEnumerable().GetEnumerator();

            Assert.True(sequence.MoveNext());
            Assert.Equal(0, sequence.Current);

            Assert.True(sequence.MoveNext());
            Assert.Equal(1, sequence.Current);

            Assert.True(sequence.MoveNext());
            Assert.Equal(1, sequence.Current);

            Assert.True(sequence.MoveNext());
            Assert.Equal(2, sequence.Current);

            Assert.True(sequence.MoveNext());
            Assert.Equal(3, sequence.Current);

            Assert.True(sequence.MoveNext());
            Assert.Equal(5, sequence.Current);

            Assert.True(sequence.MoveNext());
            Assert.Equal(8, sequence.Current);

            Assert.True(sequence.MoveNext());
            Assert.Equal(13, sequence.Current);
        }
    }
}
