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

        public static void EqualsTest(object obj1, object obj2, bool expected)
        {
            if (obj1 != null)
            {
                Assert.AreEqual(expected, obj1.Equals(obj2));
            }
            Assert.AreEqual(expected, Equals(obj1, obj2));
        }
    }
}
