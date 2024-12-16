using System;
using System.Collections;

namespace CoreLib
{
    public static class ArrayListTests
    {
        public static void Ctor_Empty()
        {
            var arrayList = new ArrayList();
            Assert.AreEqual(0, arrayList.Count);
            Assert.AreEqual(0, arrayList.Capacity);
        }

        public static void Ctor_Int(int capacity)
        {
            var arrayList = new ArrayList(capacity);
            Assert.AreEqual(capacity, arrayList.Capacity);
        }

        public static void Add_SmallCapacity(int count)
        {
            var arrayList = new ArrayList(1);
            for (int i = 0; i < count; i++)
            {
                arrayList.Add(i);
                Assert.AreEqual(i, arrayList[i]);
                Assert.AreEqual(i + 1, arrayList.Count);
                Assert.IsTrue(arrayList.Capacity >= arrayList.Count);
            }

            Assert.AreEqual(count, arrayList.Count);

            for (int i = 0; i < count; i++)
            {
                arrayList.RemoveAt(0);
            }

            Assert.AreEqual(0, arrayList.Count);
        }

        public static void IndexOf_Int()
        {
            var data = new int[10];
            var arrayList = new ArrayList(10);
            for (int i = 0; i < 10; i ++)
            {
                data[i] = i;
                arrayList.Add(i);
            }

            foreach (var item in data)
            {
                Assert.AreEqual(item, arrayList.IndexOf(item));
            }
        }

        public static void IndexOf_NonExistentObject()
        {
            var arrayList = new ArrayList(3);
            arrayList.Add(1);
            arrayList.Add(2);
            arrayList.Add(3);

            Assert.AreEqual(-1, arrayList.IndexOf(null));
            Assert.AreEqual(-1, arrayList.IndexOf("wibble"));
            Assert.AreEqual(-1, arrayList.IndexOf(5));
        }

        public static void GetEnumerator_Int()
        {
            var arrayList = new ArrayList(3);
            arrayList.Add(1);
            arrayList.Add(2);
            arrayList.Add(3);

            var enumerator = arrayList.GetEnumerator();
            for (int i = 1; i <= 3; i++)
            {
                enumerator.MoveNext();
                Assert.AreEqual(i, enumerator.Current);
            }
        }

        public static void Remove_Int()
        {
            var arrayList = new ArrayList();
            arrayList.Add(0);
            arrayList.Add(1);
            arrayList.Add(2);

            arrayList.Remove(0);
            arrayList.Remove(1);
            arrayList.Remove(2);

            Assert.AreEqual(0, arrayList.Count);
        }

        public static void Clear_Int()
        {
            var arrayList = new ArrayList();
            arrayList.Add(0);
            arrayList.Add(1);
            arrayList.Add(2);
            arrayList.Add(3);

            arrayList.Clear();

            Assert.AreEqual(0, arrayList.Count);
        }

        public static void Contains_Int()
        {
            var arrayList = new ArrayList();
            arrayList.Add(0);
            arrayList.Add(1);
            arrayList.Add(2);

            Assert.IsTrue(arrayList.Contains(2));
        }

        public static void Indexer_Int()
        {
            var arrayList = new ArrayList();
            arrayList.Add(0);
            arrayList.Add(1);
            arrayList.Add(2);

            arrayList[1] = arrayList[0];

            Assert.AreEqual(arrayList[0], arrayList[1]);   
        }

        public static void RunArrayListTests()
        {
            Ctor_Empty();
            Ctor_Int(2);
            Ctor_Int(16);
            Add_SmallCapacity(10);
            IndexOf_Int();
            IndexOf_NonExistentObject();
            Remove_Int();
            Clear_Int();
            Contains_Int();
            Indexer_Int();
            GetEnumerator_Int();
        }
    }
}
