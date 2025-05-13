using Xunit;
using System.Collections.Generic;

namespace System.Collections.Tests
{
    public abstract class IEnumerable_Generic_Tests<T>
    {
        protected abstract IEnumerable<T> GenericIEnumerableFactory(int count);

        private void GetEnumerator_ReturnsNotNull(int count)
        {
            var enumerable = GenericIEnumerableFactory(count);
            Assert.NotNull(enumerable.GetEnumerator());
        }

        private void GetEnumerator_ReturnsUniqueEnumerator(int count)
        {
            var enumerable = GenericIEnumerableFactory(count);
            int iterations = 0;

            foreach (var item in enumerable)
                foreach (var item2 in enumerable)
                    foreach (var item3 in enumerable)
                        iterations++;

            Assert.Equal(count * count * count, iterations);
        }

        private void MoveNext_FromStartToFinish(int count)
        {
            using (var enumerator = GenericIEnumerableFactory(count).GetEnumerator())
            {
                int iterations = 0;
                while (enumerator.MoveNext())
                    iterations++;

                Assert.Equal(count, iterations);
            }
        }

        private void MoveNext_AfterEndOfCollection(int count)
        {
            using (var enumerator = GenericIEnumerableFactory(count).GetEnumerator())
            {
                for (int i = 0; i < count; i++)
                    enumerator.MoveNext();

                Assert.False(enumerator.MoveNext());
                Assert.False(enumerator.MoveNext());
            }
        }

        private void Current_FromStartToFinish(int count)
        {
            using (var enumerator = GenericIEnumerableFactory(count).GetEnumerator())
            {
                while (enumerator.MoveNext())
                    _ = enumerator.Current;
            }
        }

        private void Current_ReturnsSameValueOnSubsequentCalls(int count)
        {
            using (var enumerator = GenericIEnumerableFactory(count).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current!;
                    Assert.Equal(current, enumerator.Current!);
                    Assert.Equal(current, enumerator.Current!);
                    Assert.Equal(current, enumerator.Current!);
                }
            }
        }

        private void Reset_BeforeIteration(int count)
        {
            var enumerator = GenericIEnumerableFactory(count).GetEnumerator();
            enumerator.Reset();
        }

        private void Reset_DuringIteration(int count)
        {
            using (var enumerator = GenericIEnumerableFactory(count).GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    var current = enumerator.Current!;
                    enumerator.Reset();
                    if (enumerator.MoveNext())
                    {
                        Assert.Equal(current, enumerator.Current!);
                    }
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
