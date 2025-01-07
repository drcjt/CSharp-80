namespace System.Tests
{
    internal static class ObjectTests
    {
        public static void EqualsTests()
        {
            var obj1 = new object();
            var obj2 = new object();

            EqualsTest(obj1, obj1, true);
            EqualsTest(obj1, null, false);
            EqualsTest(obj1, obj2, false);

            EqualsTest(null, null, true);
            EqualsTest(null, obj1, false);
        }

        public static void EqualsTest(object? obj1, object? obj2, bool expected)
        {
            if (obj1 != null)
            {
                Assert.AreEqual(expected, obj1.Equals(obj2));
            }
            Assert.AreEqual(expected, Equals(obj1, obj2));
        }

        public static void ReferenceEqualsTests()
        {
            var equalsTester1 = new EqualsTester(7);
            var equalsTester2 = new EqualsTester(8);

            EqualsTester.EqualsCalled = false;
            Assert.IsFalse(ReferenceEquals(equalsTester1, equalsTester2));
            Assert.IsFalse(EqualsTester.EqualsCalled);

            EqualsTester.EqualsCalled = false;
            Assert.IsTrue(ReferenceEquals(equalsTester1, equalsTester1));
            Assert.IsFalse(EqualsTester.EqualsCalled);

            EqualsTester.EqualsCalled = false;
            Assert.IsFalse(ReferenceEquals(equalsTester1, null));
            Assert.IsFalse(EqualsTester.EqualsCalled);

            EqualsTester.EqualsCalled = false;
            Assert.IsFalse(ReferenceEquals(null, equalsTester1));
            Assert.IsFalse(EqualsTester.EqualsCalled);
        }

        private sealed class EqualsTester(int x)
        {
            public int X = x;
            public static bool EqualsCalled = false;

            public override bool Equals(object? obj)
            {
                EqualsCalled = true;

                EqualsTester? et = obj as EqualsTester;
                if (et == null) return false;
                return et.X == X;
            }

            public override int GetHashCode() => 42;
        }
    }
}
