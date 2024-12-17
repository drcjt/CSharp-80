using System.Collections.Generic;

namespace System.Collections.Tests
{
    public class List_Generic_Tests_int : List_Generic_Tests<int>
	{
        private readonly Random _random = new Random();

        protected override int CreateT(int seed)
        {
            return _random.Next();
        }

        private readonly IList<int> _list = new List<int>();

        protected override IList<int> GenericIListFactory()
        {
            _list.Clear();
            return _list;
        }
    }

    public class List_Generic_Tests_string : List_Generic_Tests<string>
    {
        protected override string CreateT(int seed)
        {
            return seed.ToString();
        }

        private readonly IList<string> _list = new List<string>();

        protected override IList<string> GenericIListFactory()
        {
            _list.Clear();
            return _list;
        }
    }

    public abstract class List_Generic_Tests<T> : IList_Generic_Tests<T>
	{
        public void RunTests()
        {
            var validCollectionSizes = new int[] { 0, 1, 5 };

            foreach (int size in validCollectionSizes)
                RunTests(size);
        }
	}
}
