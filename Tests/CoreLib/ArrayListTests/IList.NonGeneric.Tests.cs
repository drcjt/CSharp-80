using System.Collections.Generic;

namespace System.Collections.Tests
{
    public abstract class IList_NonGeneric_Tests : ICollection_NonGeneric_Tests
    {
        protected abstract IList NonGenericIListFactory();

        protected virtual IList NonGenericIListFactory(int count)
        {
            var list = NonGenericIListFactory();
            AddToCollection(list, count);
            return list;
        }

        protected override void AddToCollection(ICollection collection, int count)
        {
            var list = collection as IList;
            if (list != null)
                AddToCollection(list, count);
        }

        protected virtual void AddToCollection(IList collection, int count)
        {
            int seed = 0;
            while (collection.Count < count)
            {
                object toAdd = CreateT(seed++);
                while (collection.Contains(toAdd))
                    toAdd = CreateT(seed++);
                collection.Add(toAdd);
            }
        }

        public override void RunTests(int size)
        {
            base.RunTests(size);

            ItemGet_ValidGetWithinListBounds(size);

            ItemSet_FirstItemToNonNull(size);
            ItemSet_FirstItemToNull(size);
            ItemSet_LastItemToNonNull(size);
            ItemSet_LastItemToNull(size);
            ItemSet_DuplicateValues(size);

            Add_Null(size);
            Add_DuplicateValues(size);
            Add_AfterCallingClear(size);
            Add_AfterRemovingAnyValue(size);
            Add_AfterRemovingAllItems(size);
            Add_AfterRemoving(size);

            Clear(size);

            Contains_ValueNotInListNotContainingValue(size);
            Contains_ValueInListContainingValue(size);
            Contains_NullNotInListNotContainingNull(size);
            Contains_NullInListContainingNull(size);

            IndexOf_NullNotContainedInList(size);
            IndexOf_NullContainedInList(size);
            IndexOf_ValueInCollectionMultipleTimes(size);
            IndexOf_EachValueNoDuplicates(size);
            IndexOf_ReturnsFirstMatchingValue(size);

            Insert_IndexGreaterThanListCount_Appends(size);
            Insert_FirstItemToNonNull(size);
            Insert_FirstItemToNull(size);
            Insert_LastItemToNonNull(size);
            Insert_LastItemToNull(size);
            Insert_DuplicateValues(size);

            Remove_NullNotContainedInList(size);
            Remove_NonNullNotContainedInList(size);
            Remove_NullContainedInList(size);
            Remove_NonNullContainedInList(size);
            Remove_ValueThatExistsTwiceInList(size);
            Remove_AllValues(size);

            RemoveAt_AllIndices(size);
            RemoveAt_ZeroMultipleTimes(size);

            CurrentAtEnd_AfterAdd(size);
        }

        protected override ICollection NonGenericICollectionFactory() => NonGenericIListFactory();

        protected override ICollection NonGenericICollectionFactory(int count) => NonGenericIListFactory(count);

        protected virtual object CreateT(int seed)
        {
            if (seed % 2 == 0)
            {
                var ch = (char)(0x41 + (seed % 64));
                return new string([ch]);
            }
            else
            {
                Random rand = new Random(seed);
                return rand.Next();
            }
        }

        private object CreateTNotInList(int seed, IList list)
        {
            var item = CreateT(seed++);
            while (list.Contains(item))
                item = CreateT(seed++);
            return item;
        }

        private void ItemGet_ValidGetWithinListBounds(int count)
        {
            var list = NonGenericIListFactory(count);
            for (int i = 0; i <  count; i++)
            {
                _ = list[i];
            }
        }        

        private void ItemSet_FirstItemToNonNull(int count)
        {
            if (count > 0)
            {
                var list = NonGenericIListFactory(count);
                object value = CreateT(0);
                list[0] = value;
                Assert.AreEqual(value, list[0]);
            }
        }

        private void ItemSet_FirstItemToNull(int count)
        {
            if (count > 0)
            {
                var list = NonGenericIListFactory(count);
                object? value = null;
                list[0] = value;
                Assert.AreEqual(value, list[0]);
            }            
        }

        private void ItemSet_LastItemToNonNull(int count)
        {
            if (count > 0)
            {
                var list = NonGenericIListFactory(count);
                object value = CreateT(0);
                var lastIndex = count - 1;
                list[lastIndex] = value;
                Assert.AreEqual(value, list[lastIndex]);
            }
        }

        private void ItemSet_LastItemToNull(int count)
        {
            if (count > 0)
            {
                var list = NonGenericIListFactory(count);
                object? value = null;
                var lastIndex = count - 1;
                list[lastIndex] = value;
                Assert.AreEqual(value, list[lastIndex]);
            }
        }

        private void ItemSet_DuplicateValues(int count)
        {
            if (count > 2)
            {
                var list = NonGenericIListFactory(count);
                object value = CreateT(0);
                list[0] = value;
                list[1] = value;
                Assert.AreEqual(value, list[0]);
                Assert.AreEqual(value, list[1]);
            }
        }

        private void Add_Null(int count)
        {
            var list = NonGenericIListFactory(count);
            list.Add(null);
            Assert.AreEqual(count + 1, list.Count);
        }

        private void Add_DuplicateValues(int count)
        {
            var list = NonGenericIListFactory(count);
            object duplicateValue = CreateT(0);
            list.Add(duplicateValue);
            list.Add(duplicateValue);
            Assert.AreEqual(count + 2, list.Count);
        }

        private void Add_AfterCallingClear(int count)
        {
            var list = NonGenericIListFactory(count);
            list.Clear();
            AddToCollection(list, 5);
            Assert.AreEqual(5, list.Count);
        }

        private void Add_AfterRemovingAnyValue(int count)
        {
            var list = NonGenericIListFactory(count);
            var toAdd = CreateT(0);
            list.Add(toAdd);
            list.RemoveAt(0);

            toAdd = CreateT(0);
            list.Add(toAdd);
        }

        private void Add_AfterRemovingAllItems(int count)
        {
            var list = NonGenericIListFactory(count);
            object?[] arr = new object[count];
            for (int i = 0; i < count; i++)
                arr[i] = list[i];
            for (int i = 0; i < count; i++)
                list.Remove(arr[i]);
            list.Add(CreateT(0));
            Assert.AreEqual(1, list.Count);
        }

        private void Add_AfterRemoving(int count)
        {
            var list = NonGenericIListFactory(count);
            object toAdd = CreateT(0);
            list.Add(toAdd);
            list.Remove(toAdd);
            list.Add(toAdd);
        }

        private void Clear(int count)
        {
            var list = NonGenericIListFactory(count);
            list.Clear();
            Assert.AreEqual(0, list.Count);
        }

        private void Contains_ValueNotInListNotContainingValue(int count)
        {
            var list = NonGenericIListFactory(count);
            var item = CreateTNotInList(0, list);
            Assert.IsFalse(list.Contains(item));
        }

        private void Contains_ValueInListContainingValue(int count)
        {
            var list = NonGenericIListFactory(count);
            foreach (var item in list)
                Assert.IsTrue(list.Contains(item));
        }

        private void Contains_NullNotInListNotContainingNull(int count)
        {
            var list = NonGenericIListFactory(count);
            Assert.IsFalse(list.Contains(null));
        }

        private void Contains_NullInListContainingNull(int count)
        {
            var list = NonGenericIListFactory(count);
            list.Add(null);
            Assert.IsTrue(list.Contains(null));
        }

        private void IndexOf_NullNotContainedInList(int count)
        {
            var list = NonGenericIListFactory(count);
            Assert.AreEqual(-1, list.IndexOf(null));
        }

        private void IndexOf_NullContainedInList(int count)
        {
            if (count > 0)
            {
                var list = NonGenericIListFactory(count);
                list[0] = null;
                Assert.AreEqual(0, list.IndexOf(null));
            }
        }

        private void IndexOf_ValueInCollectionMultipleTimes(int count)
        {
            if (count > 0)
            {
                var list = NonGenericIListFactory(count);
                var value = CreateT(0);
                list[0] = value;
                list[count / 2] = value;
                Assert.AreEqual(0, list.IndexOf(value));
            }
        }

        private void IndexOf_EachValueNoDuplicates(int count)
        {
            var list = NonGenericIListFactory(count);
            for (int i = 0; i < count; i++)
            {
                Assert.AreEqual(i, list.IndexOf(list[i]));
            }
        }

        private void IndexOf_ReturnsFirstMatchingValue(int count)
        {
            var list = NonGenericIListFactory(count);
            var originalItems = new object?[count];

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
            var list = NonGenericIListFactory(count);
            var toInsert = CreateT(0);
            list.Insert(count, toInsert);
            Assert.AreEqual(count + 1, list.Count);
            Assert.AreEqual(toInsert, list[count]);
        }

        private void Insert_FirstItemToNonNull(int count)
        {
            var list = NonGenericIListFactory(count);
            var toInsert = CreateT(0);
            list.Insert(0, toInsert);
            Assert.AreEqual(toInsert, list[0]);
            Assert.AreEqual(count + 1, list.Count);
        }

        private void Insert_FirstItemToNull(int count)
        {
            var list = NonGenericIListFactory(count);
            list.Insert(0, null);
            Assert.AreEqual(null, list[0]);
            Assert.AreEqual(count + 1, list.Count);
        }

        private void Insert_LastItemToNonNull(int count)
        {
            var list = NonGenericIListFactory(count);
            var toInsert = CreateT(0);
            int lastIndex = count > 0 ? count - 1 : 0;
            list.Insert(lastIndex, toInsert);
            Assert.AreEqual(toInsert, list[lastIndex]);
            Assert.AreEqual(count + 1, list.Count);
        }

        private void Insert_LastItemToNull(int count)
        {
            var list = NonGenericIListFactory(count);
            int lastIndex = count > 0 ? count - 1 : 0;
            list.Insert(lastIndex, null);
            Assert.AreEqual(null, list[lastIndex]);
            Assert.AreEqual(count + 1, list.Count);
        }

        private void Insert_DuplicateValues(int count)
        {
            var list = NonGenericIListFactory(count);
            var toInsert = CreateT(0);
            list.Insert(0, toInsert);
            list.Insert(1, toInsert);
            Assert.AreEqual(toInsert, list[0]);
            Assert.AreEqual(toInsert, list[1]);
            Assert.AreEqual(count + 2, list.Count);
        }

        private void Remove_NullNotContainedInList(int count)
        {
            var list = NonGenericIListFactory(count);
            list.Remove(null);
            Assert.AreEqual(count, list.Count);
        }

        private void Remove_NonNullNotContainedInList(int count)
        {
            var list = NonGenericIListFactory(count);
            var item = CreateTNotInList(0, list);
            list.Remove(item);
            Assert.AreEqual(count, list.Count);
        }

        private void Remove_NullContainedInList(int count)
        {
            var list = NonGenericIListFactory(count);
            list.Add(null);
            list.Remove(null);
            Assert.AreEqual(count, list.Count);
        }

        private void Remove_NonNullContainedInList(int count)
        {
            var list = NonGenericIListFactory(count);
            var item = CreateT(0);
            list.Add(item);
            list.Remove(item);
            Assert.AreEqual(count, list.Count);
        }

        private void Remove_ValueThatExistsTwiceInList(int count)
        {
            var list = NonGenericIListFactory(count);
            var item = CreateT(0);
            list.Add(item);
            list.Add(item);
            list.Remove(item);
            Assert.AreEqual(count + 1, list.Count);
        }

        private void Remove_AllValues(int count)
        {
            var list = NonGenericIListFactory(count);
            var items = new object?[count];
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
            var list = NonGenericIListFactory(count);
            for (int index = count - 1; index >= 0; index--)
            {
                list.RemoveAt(index);
                Assert.AreEqual(index, list.Count);
            }
        }

        private void RemoveAt_ZeroMultipleTimes(int count)
        {
            var list = NonGenericIListFactory(count);
            for (int index = 0; index < count; index++)
            {
                list.RemoveAt(0);
                Assert.AreEqual(count - index - 1, list.Count);
            }
        }

        private void CurrentAtEnd_AfterAdd(int count)
        {
            var list = NonGenericIListFactory(count);
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