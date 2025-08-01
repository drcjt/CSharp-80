using System.Collections.Generic;
using Xunit;

namespace System.Collections.Tests
{
    static class EqualityComparerTests
    {
        public class DefaultType { }

        [Fact]
        public static void Default_ForType_CreatesNoMoreThanOneComparerInstance()
        {
            _ = EqualityComparer<DefaultType>.Default;
            var afterComparerCreation = GC.GetTotalMemory();
            for (int i = 0; i < 10; i++)
            {
                _ = EqualityComparer<DefaultType>.Default;
            }
            var afterAllComparerRequests = GC.GetTotalMemory();
            Assert.Equal(afterComparerCreation, afterAllComparerRequests);
        }

        [Fact]
        public static void EqualsTests()
        {
            EqualsTest<byte>(3, 3, true);
            EqualsTest<byte>(3, 4, false);
            EqualsTest<byte>(0, 255, false);
            EqualsTest<byte>(0, 128, false);
            EqualsTest<byte>(255, 255, true);

            EqualsTest<Int32>(3, 3, true);
            EqualsTest<Int32>(3, 5, false);
            EqualsTest<Int32>(int.MinValue + 1, 1, false);
            EqualsTest<Int32>(int.MinValue, int.MinValue, true);

            Equatable one = new Equatable(1);

            EqualsTest<Equatable>(one, one, true);
            EqualsTest<Equatable>(one, new Equatable(1), true);
            EqualsTest<Equatable>(new Equatable(int.MinValue + 1), new Equatable(1), false);
            EqualsTest<Equatable>(new Equatable(-1), new Equatable(int.MaxValue), false);

            var obj = new object();

            EqualsTest<object>(obj, obj, true);
            EqualsTest<object>(obj, new object(), false);
            EqualsTest<object>(obj, null, false);
        }

        private static void EqualsTest<T>(T? left, T? right, bool expected)
        {
            var comparer = EqualityComparer<T>.Default;

            // Commutative tests
            Assert.Equal(expected, comparer.Equals(left, right));
            Assert.Equal(expected, comparer.Equals(right, left));

            // Reflexive tests
            Assert.True(comparer.Equals(left, left));
            Assert.True(comparer.Equals(right, right));

            if (default(T) is null)
            {
                T? nil = default;

                Assert.True(comparer.Equals(nil, nil));

                Assert.Equal(left is null, comparer.Equals(left, nil));
                Assert.Equal(left is null, comparer.Equals(nil, left));

                Assert.Equal(right is null, comparer.Equals(right, nil));
                Assert.Equal(right is null, comparer.Equals(nil, right));
            }
        }
    }
}
