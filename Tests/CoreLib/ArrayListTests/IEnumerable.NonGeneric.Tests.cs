namespace System.Collections.Tests
{
    public abstract class IEnumerable_NonGeneric_Tests
    {
        protected abstract IEnumerable NonGenericIEnumerableFactory(int count);

        private void GetEnumerator_ReturnsNotNull(int count)
        {
            var enumerable = NonGenericIEnumerableFactory(count);
            Assert.IsNotNull(enumerable.GetEnumerator());
        }

        private void GetEnumerator_ReturnsUniqueEnumerator(int count)
        {
            var enumerable = NonGenericIEnumerableFactory(count);
            int iterations = 0;

            foreach (var item in enumerable)
                foreach (var item2 in enumerable)
                    foreach (var item3 in enumerable)
                        iterations++;

            Assert.AreEquals(count * count * count, iterations);
        }

        private void MoveNext_FromStartToFinish(int count)
        {
            var enumerator = NonGenericIEnumerableFactory(count).GetEnumerator();
            int iterations = 0;
            while (enumerator.MoveNext())
                iterations++;

            Assert.AreEquals(count, iterations);
        }

        private void MoveNext_AfterEndOfCollection(int count)
        {
            var enumerator = NonGenericIEnumerableFactory(count).GetEnumerator();
            for (int i = 0; i < count; i++)
                enumerator.MoveNext();

            Assert.IsFalse(enumerator.MoveNext());
            Assert.IsFalse(enumerator.MoveNext());
        }

        private void Current_FromStartToFinish(int count)
        {
            var enumerator = NonGenericIEnumerableFactory(count).GetEnumerator();
            while (enumerator.MoveNext())
                _ = enumerator.Current;
        }

        private void Current_ReturnsSameValueOnSubsequentCalls(int count)
        {
            var enumerator = NonGenericIEnumerableFactory(count).GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                Assert.AreEquals(current, enumerator.Current);
                Assert.AreEquals(current, enumerator.Current);
                Assert.AreEquals(current, enumerator.Current);
            }
        }

        private void Reset_BeforeIteration(int count)
        {
            var enumerator = NonGenericIEnumerableFactory(count).GetEnumerator();
            enumerator.Reset();
        }

        private void Reset_DuringIteration(int count)
        {
            var enumerator = NonGenericIEnumerableFactory(count).GetEnumerator();
            if (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                enumerator.Reset();
                if (enumerator.MoveNext())
                {
                    Assert.AreEquals(current, enumerator.Current);
                }
            }
        }

        public virtual void RunTests(int size)
        {
            GetEnumerator_ReturnsNotNull(size);
            GetEnumerator_ReturnsUniqueEnumerator(size);

            MoveNext_FromStartToFinish(size);
            MoveNext_AfterEndOfCollection(size);

            Current_FromStartToFinish(size);
            Current_ReturnsSameValueOnSubsequentCalls(size);

            Reset_BeforeIteration(size);
            Reset_DuringIteration(size);
        }
    }
}
