using Xunit;

namespace System.Tests
{
    public static class RandomTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static void SmallRanges_ReturnsExpectedValues(bool seeded)
        {
            Random r = Create(seeded);

            Assert.Equal(0, r.Next(0));
            Assert.Equal(0, r.Next(0, 0));
            Assert.Equal(1, r.Next(1, 1));

            Assert.Equal(0, r.Next(1));
            Assert.Equal(1, r.Next(1, 2));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static void NextInt_AllValuesAreWithinSpecifiedRange(bool seeded)
        {
            Random r = Create(seeded);

            for (int i = 0; i < 100; i++)
            {
                Assert.InRange(r.Next(20), 0, 19);
                Assert.InRange(r.Next(20, 30), 20, 29);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static void Next_Int_AllValuesWithinSmallRangeHit(bool seeded)
        {
            Random r = Create(seeded);


            bool[] seen = new bool[4];
            for (int i = 0; i < 100; i++)
            {
                seen[r.Next(4)] = true;
            }

            for (int i = 0; i < seen.Length; i++)
            {
                Assert.True(seen[i]);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static void Next_IntInt_AllValuesWithinSmallRangeHit(bool seeded)
        {
            Random r = Create(seeded);

            bool[] seen = new bool[2];
            for (int i = 0; i < 100; i++)
            {
                seen[r.Next(42, 44) - 42] = true;
            }

            for (int i = 0; i < 2; i++)
            {
                Assert.True(seen[i]);
            }
        }

        [Fact]
        public static void CtorWithSeed_SequenceIsRepeatable()
        {
            Random r1 = Create(seeded: false);
            Random r2 = Create(seeded: false);

            for (int i = 0; i < 100; i++)
            {
                Assert.Equal(r1.Next(), r2.Next());
            }
        }

        private static Random Create(bool seeded) =>
            seeded switch
            {
                false => new Random(),
                true => new Random(42),
            };
    }
}
