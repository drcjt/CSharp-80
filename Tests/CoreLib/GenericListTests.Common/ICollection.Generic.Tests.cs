using System.Collections.Generic;

namespace System.Collections.Tests
{
    public abstract class ICollection_Generic_Tests<T> : IEnumerable_Generic_Tests<T>
    {
        protected abstract ICollection<T> GenericICollectionFactory();

        protected virtual ICollection<T> GenericICollectionFactory(int count)
        {
            var collection = GenericICollectionFactory();

            AddToCollection(collection, count);

            return collection;
        }

        protected abstract T CreateT(int seed);

        protected virtual void AddToCollection(ICollection<T> collection, int count)
        {
            int seed = 0;
            while (collection.Count < count)
            {
                T toAdd = CreateT(seed++);

                while (collection.Contains(toAdd))
                    toAdd = CreateT(seed++);

                collection.Add(toAdd);
            }
        }

        protected override IEnumerable<T> GenericIEnumerableFactory(int count) => GenericICollectionFactory(count);

        private void Count_Validity(int count)
        {
            var collection = GenericICollectionFactory(count);
            Assert.AreEqual(count, collection.Count);
        }

        private void CopyTo_ExactlyEnoughSpaceInArray(int count)
        {
            var collection = GenericICollectionFactory(count);
            T[] array = new T[count];
            collection.CopyTo(array, 0);
            int i = 0;
            foreach (var item in collection)
            {
                Assert.AreEqual(array[i++], item);
            }
        }

        public override void RunTests(int size)
        {
            base.RunTests(size);

            Count_Validity(size);
            //CopyTo_ExactlyEnoughSpaceInArray(size);
        }
    }
}
