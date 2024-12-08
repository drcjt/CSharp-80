namespace System.Collections.Tests
{
    public abstract class ICollection_NonGeneric_Tests : IEnumerable_NonGeneric_Tests
    {
        protected abstract ICollection NonGenericICollectionFactory();

        protected virtual ICollection NonGenericICollectionFactory(int count)
        {
            var collection = NonGenericICollectionFactory();
            AddToCollection(collection, count);
            return collection;
        }

        protected abstract void AddToCollection(ICollection collection, int count);

        protected override IEnumerable NonGenericIEnumerableFactory(int count) => NonGenericICollectionFactory(count);

        private void Count_Validity(int count)
        {
            var collection = NonGenericICollectionFactory(count);
            Assert.AreEquals(count, collection.Count);
        }

        private void CopyTo_ExactlyEnoughSpaceInArray(int count)
        {
            var collection = NonGenericICollectionFactory(count);
            object[] array = new object[count];
            collection.CopyTo(array, 0);
            int i = 0;
            foreach (var item in collection)
            {
                Assert.AreEquals(array[i++], item);
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
