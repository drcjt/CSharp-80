using Xunit;

namespace System.Tests
{
    internal static class NullableTests
    {
        public static void BasicTests()
        {
            //int? n = default(int?);
            /*
            Assert.IsFalse(n.HasValue);
            Assert.IsTrue(n is null);
            Assert.IsTrue(7 != n);
            Assert.AreEqual(0, n.GetHashCode());
            Assert.AreEqual("", n.ToString());
            Assert.AreEqual(default(int), n.GetValueOrDefault());
            Assert.AreEqual(999, n.GetValueOrDefault(999));
            */

            //n = new int?(42);
            /*
            Assert.IsTrue(n.HasValue);
            Assert.AreEqual(42, n.Value);
            Assert.AreEqual(42, (int)n);

            Assert.IsTrue(n is not null);
            Assert.IsTrue(7 != n);
            */
            //Assert.AreEqual(42, n);
            //Assert.IsTrue(n.Equals(42));

            /*
            Console.WriteLine(n.GetHashCode());
            Console.WriteLine(42.GetHashCode());

            Assert.IsTrue(42.GetHashCode() == n.GetHashCode());
            Assert.AreEqual(42.ToString(), n.ToString());
            Assert.AreEqual(n.GetValueOrDefault(), 42);
            Assert.AreEqual(n.GetValueOrDefault(999), 42);

            n = 88;
            Assert.IsTrue(n.HasValue);
            Assert.IsTrue(n.Equals(88));
            */
        }
    }
}
