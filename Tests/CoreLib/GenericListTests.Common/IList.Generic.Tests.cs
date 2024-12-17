using System.Collections.Generic;

namespace System.Collections.Tests
{
    public abstract class IList_Generic_Tests<T> : ICollection_Generic_Tests<T>
    {
        protected abstract IList<T> GenericIListFactory();

        protected virtual IList<T> GenericIListFactory(int count)
        {
            var list = GenericIListFactory();

            AddToCollection(list, count);
            return list;
        }

        public override void RunTests(int size)
        {
            base.RunTests(size);

            ItemGet_ValidGetWithinListBounds(size);

            ItemSet_FirstItemToNonDefaultValue(size);
            ItemSet_FirstItemToDefaultValue(size);
            ItemSet_LastItemToNonDefaultValue(size);
            ItemSet_LastItemToDefaultValue(size);
            ItemSet_DuplicateValues(size);

            Add_DefaultValue(size);
            Add_DuplicateValues(size);
            Add_AfterCallingClear(size);
            Add_AfterRemovingAnyValue(size);
            Add_AfterRemovingAllItems(size);
            Add_AfterRemoving(size);

            Clear(size);

            Contains_ValueNotInListNotContainingValue(size);
            Contains_ValueInListContainingValue(size);
            Contains_DefaultValueNotInListNotContainingDefaultValue(size);
            Contains_DefaultValueInListContainingDefaultValue(size);

            IndexOf_DefaultValueNotContainedInList(size);
            IndexOf_DefaultValueContainedInList(size);
            IndexOf_ValueInCollectionMultipleTimes(size);
            IndexOf_EachValueNoDuplicates(size);
            IndexOf_ReturnsFirstMatchingValue(size);

            Insert_IndexGreaterThanListCount_Appends(size);
            Insert_FirstItemToNonDefaultValue(size);
            Insert_FirstItemToDefaultValue(size);
            Insert_LastItemToNonDefaultValue(size);
            Insert_LastItemToDefaultValue(size);
            Insert_DuplicateValues(size);

            Remove_DefaultValueNotContainedInList(size);
            Remove_NonDefaultValueNotContainedInList(size);
            Remove_DefaultValueContainedInList(size);
            Remove_NonDefaultValueContainedInList(size);

            Remove_ValueThatExistsTwiceInList(size);
            Remove_AllValues(size);

            RemoveAt_AllIndices(size);
            RemoveAt_ZeroMultipleTimes(size);

            CurrentAtEnd_AfterAdd(size);
        }

        protected override ICollection<T> GenericICollectionFactory() => GenericIListFactory();

        protected override ICollection<T> GenericICollectionFactory(int count) => GenericIListFactory(count);

        private T CreateTNotInList(int seed, IList<T> list)
        {
            var item = CreateT(seed++);
            while (list.Contains(item))
                item = CreateT(seed++);
            return item;
        }

        private void ItemGet_ValidGetWithinListBounds(int count)
        {
            var list = GenericIListFactory(count);
            for (int i = 0; i <  count; i++)
            {
                _ = list[i];
            }
        }        

        private void ItemSet_FirstItemToNonDefaultValue(int count)
        {
            if (count > 0)
            {
                var list = GenericIListFactory(count);
                T value = CreateT(0);
                list[0] = value;
                Assert.AreEqual(value, list[0]);
            }
        }

        private void ItemSet_FirstItemToDefaultValue(int count)
        {
            if (count > 0)
            {
                var list = GenericIListFactory(count);
                list[0] = default!;
                Assert.AreEqual(default(T), list[0]);
            }            
        }

        private void ItemSet_LastItemToNonDefaultValue(int count)
        {
            if (count > 0)
            {
                var list = GenericIListFactory(count);
                T value = CreateT(0);
                var lastIndex = count - 1;
                list[lastIndex] = value;
                Assert.AreEqual(value, list[lastIndex]);
            }
        }

        private void ItemSet_LastItemToDefaultValue(int count)
        {
            if (count > 0)
            {
                var list = GenericIListFactory(count);
                var lastIndex = count - 1;
                list[lastIndex] = default!;
                Assert.AreEqual(default(T), list[lastIndex]);
            }
        }

        private void ItemSet_DuplicateValues(int count)
        {
            if (count > 2)
            {
                var list = GenericIListFactory(count);
                T value = CreateT(0);
                list[0] = value;
                list[1] = value;
                Assert.AreEqual(value, list[0]);
                Assert.AreEqual(value, list[1]);
            }
        }

        private void Add_DefaultValue(int count)
        {
            var list = GenericIListFactory(count);
            list.Add(default!);
            Assert.AreEqual(count + 1, list.Count);
        }

        private void Add_DuplicateValues(int count)
        {
            var list = GenericIListFactory(count);
            T duplicateValue = CreateT(0);
            list.Add(duplicateValue);
            list.Add(duplicateValue);
            Assert.AreEqual(count + 2, list.Count);
        }

        private void Add_AfterCallingClear(int count)
        {
            var list = GenericIListFactory(count);
            list.Clear();
            AddToCollection(list, 5);
            Assert.AreEqual(5, list.Count);
        }

        private void Add_AfterRemovingAnyValue(int count)
        {
            var list = GenericIListFactory(count);
            var toAdd = CreateT(0);
            list.Add(toAdd);
            list.RemoveAt(0);

            toAdd = CreateT(0);
            list.Add(toAdd);
        }

        private void Add_AfterRemovingAllItems(int count)
        {
            var list = GenericIListFactory(count);
            T[] arr = new T[count];
            for (int i = 0; i < count; i++)
                arr[i] = list[i];
            for (int i = 0; i < count; i++)
                list.Remove(arr[i]);
            list.Add(CreateT(0));
            Assert.AreEqual(1, list.Count);
        }

        private void Add_AfterRemoving(int count)
        {
            var list = GenericIListFactory(count);
            T toAdd = CreateT(0);
            list.Add(toAdd);
            list.Remove(toAdd);
            list.Add(toAdd);
        }

        private void Clear(int count)
        {
            var list = GenericIListFactory(count);
            list.Clear();
            Assert.AreEqual(0, list.Count);
        }

        private void Contains_ValueNotInListNotContainingValue(int count)
        {
            var list = GenericIListFactory(count);
            T item = CreateTNotInList(0, list);
            Assert.IsFalse(list.Contains(item));
        }

        private void Contains_ValueInListContainingValue(int count)
        {
            var list = GenericIListFactory(count);
            foreach (var item in list)
                Assert.IsTrue(list.Contains(item));
        }

        private void Contains_DefaultValueNotInListNotContainingDefaultValue(int count)
        {
            var list = GenericIListFactory(count);
            Assert.IsFalse(list.Contains(default!));
        }

        private void Contains_DefaultValueInListContainingDefaultValue(int count)
        {
            var list = GenericIListFactory(count);
            list.Add(default!);
            Assert.IsTrue(list.Contains(default!));
        }

        private void IndexOf_DefaultValueNotContainedInList(int count)
        {
            var list = GenericIListFactory(count);
            Assert.AreEqual(-1, list.IndexOf(default!));
        }

        private void IndexOf_DefaultValueContainedInList(int count)
        {
            if (count > 0)
            {
                var list = GenericIListFactory(count);
                list[0] = default!;
                Assert.AreEqual(0, list.IndexOf(default!));
            }
        }

        private void IndexOf_ValueInCollectionMultipleTimes(int count)
        {
            if (count > 0)
            {
                var list = GenericIListFactory(count);
                var value = CreateT(0);
                list[0] = value;
                list[count / 2] = value;
                Assert.AreEqual(0, list.IndexOf(value));
            }
        }

        private void IndexOf_EachValueNoDuplicates(int count)
        {
            var list = GenericIListFactory(count);
            for (int i = 0; i < count; i++)
            {
                Assert.AreEqual(i, list.IndexOf(list[i]));
            }
        }

        private void IndexOf_ReturnsFirstMatchingValue(int count)
        {
            var list = GenericIListFactory(count);
            var originalItems = new T[count];

            // TODO: Replace with CopyTo when its implemented
            for (int i = 0; i < count; i++)
                originalItems[i] = list[i];

            foreach (var item in originalItems)
                list.Add(item);

            for (int i = 0; i < count; i++)
            {
                Assert.AreEqual(i, list.IndexOf(originalItems[i]));
            }
        }

        private void Insert_IndexGreaterThanListCount_Appends(int count)
        {
            var list = GenericIListFactory(count);
            var toInsert = CreateT(0);
            list.Insert(count, toInsert);
            Assert.AreEqual(count + 1, list.Count);
            Assert.AreEqual(toInsert, list[count]);
        }

        private void Insert_FirstItemToNonDefaultValue(int count)
        {
            var list = GenericIListFactory(count);
            var toInsert = CreateT(0);
            list.Insert(0, toInsert);
            Assert.AreEqual(count + 1, list.Count);
            Assert.AreEqual(toInsert, list[0]);
        }

        private void Insert_FirstItemToDefaultValue(int count)
        {
            var list = GenericIListFactory(count);
            list.Insert(0, default!);
            Assert.AreEqual(default(T), list[0]);
            Assert.AreEqual(count + 1, list.Count);
        }

        private void Insert_LastItemToNonDefaultValue(int count)
        {
            var list = GenericIListFactory(count);
            var toInsert = CreateT(0);
            int lastIndex = count > 0 ? count - 1 : 0;
            list.Insert(lastIndex, toInsert);
            Assert.AreEqual(count + 1, list.Count);
            Assert.AreEqual(toInsert, list[lastIndex]);
        }

        private void Insert_LastItemToDefaultValue(int count)
        {
            var list = GenericIListFactory(count);
            int lastIndex = count > 0 ? count - 1 : 0;
            list.Insert(lastIndex, default!);
            Assert.AreEqual(default(T), list[lastIndex]);
            Assert.AreEqual(count + 1, list.Count);
        }

        private void Insert_DuplicateValues(int count)
        {
            var list = GenericIListFactory(count);
            var toInsert = CreateT(0);
            list.Insert(0, toInsert);
            list.Insert(1, toInsert);
            Assert.AreEqual(toInsert, list[0]);
            Assert.AreEqual(toInsert, list[1]);
            Assert.AreEqual(count + 2, list.Count);
        }

        private void Remove_DefaultValueNotContainedInList(int count)
        {
            var list = GenericIListFactory(count);
            list.Remove(default!);
            Assert.AreEqual(count, list.Count);
        }

        private void Remove_NonDefaultValueNotContainedInList(int count)
        {
            var list = GenericIListFactory(count);
            var item = CreateTNotInList(0, list);
            list.Remove(item);
            Assert.AreEqual(count, list.Count);
        }

        private void Remove_DefaultValueContainedInList(int count)
        {
            var list = GenericIListFactory(count);
            list.Add(default!);
            list.Remove(default!);
            Assert.AreEqual(count, list.Count);
        }

        private void Remove_NonDefaultValueContainedInList(int count)
        {
            var list = GenericIListFactory(count);
            var item = CreateT(0);
            list.Add(item);
            list.Remove(item);
            Assert.AreEqual(count, list.Count);
        }

        private void Remove_ValueThatExistsTwiceInList(int count)
        {
            var list = GenericIListFactory(count);
            var item = CreateT(0);
            list.Add(item);
            list.Add(item);
            list.Remove(item);
            Assert.AreEqual(count + 1, list.Count);
        }

        private void Remove_AllValues(int count)
        {
            var list = GenericIListFactory(count);
            var items = new T[count];
            for (int i = 0; i < count; i++)
                items[i] = list[i];
            for (int i = 0; i < count; i++)
            {
                list.Remove(items[i]);
            }
            Assert.AreEqual(0, list.Count);
        }

        private void RemoveAt_AllIndices(int count)
        {
            var list = GenericIListFactory(count);
            for (int index = count - 1; index >= 0; index--)
            {
                list.RemoveAt(index);
                Assert.AreEqual(index, list.Count);
            }
        }

        private void RemoveAt_ZeroMultipleTimes(int count)
        {
            var list = GenericIListFactory(count);
            for (int index = 0; index < count; index++)
            {
                list.RemoveAt(0);
                Assert.AreEqual(count - index - 1, list.Count);
            }
        }

        private void CurrentAtEnd_AfterAdd(int count)
        {
            var list = GenericIListFactory(count);
            var enumerator = list.GetEnumerator();

            while (enumerator.MoveNext()) { /* Move to end of enumerator */ }

            // Current shouldn't fail
            if (count > 0)
                _ = enumerator.Current;

            // Test after adding items
            for (int i = 0; i < 3; i++)
            {
                list.Add(CreateT(0));

                // Current shouldn't fail
                if (count > 0)
                    _ = enumerator.Current;
            }
        }
    }
}