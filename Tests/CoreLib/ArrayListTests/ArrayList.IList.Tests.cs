namespace System.Collections.Tests
{
    public class ArrayListBasicTests : ArrayListIListTestBase
	{
        protected override IList NonGenericIListFactory() => new ArrayList();
    }

    public abstract class ArrayListIListTestBase : IList_NonGeneric_Tests
	{
        public void RunTests()
        {
            var validCollectionSizes = new int[] { 0, 1, 10 };

            foreach (int size in validCollectionSizes)
                RunTests(size);
        }
	}
}
