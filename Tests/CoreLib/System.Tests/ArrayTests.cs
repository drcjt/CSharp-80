using System.Collections;

namespace System.Tests
{
    internal static class ArrayTests
    {
        public static void GetValue_RankOneInt_SetValue()
        {
            var intArray = new int[] { 7, 8, 9 };
            Array array = intArray;

            Assert.AreEqual(7, array.GetValue(0));
            intArray[0] = 41;
            Assert.AreEqual(41, array.GetValue(0));

            Assert.AreEqual(8, array.GetValue(1));
            intArray[1] = 42;
            Assert.AreEqual(42, array.GetValue(1));

            Assert.AreEqual(9, array.GetValue(2));
            intArray[2] = 43;
            Assert.AreEqual(43, array.GetValue(2));
        }

        public static void GetEnumerator()
        {
            GetEnumerator(new int[0]);
            GetEnumerator(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            GetEnumerator(new char[] { 'a', 'b', 'c' });
        }

        public static void GetEnumerator(Array array)
        {
            var enumerator1 = array.GetEnumerator();
            var enumerator2 = array.GetEnumerator();
            Assert.IsTrue(enumerator1 != enumerator2);

            IEnumerator enumerator = array.GetEnumerator();
            for (int i = 0; i < 2; i++)
            {
                int counter = 0;
                while (enumerator.MoveNext())
                {
                    Assert.AreEqual(array[counter], enumerator.Current);
                    counter++;
                }

                Assert.IsFalse(enumerator.MoveNext());
                Assert.AreEqual(array.Length, counter);

                enumerator.Reset();
            }
        }
    }
}
