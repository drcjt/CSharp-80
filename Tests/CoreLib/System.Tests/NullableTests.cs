using Xunit;

namespace System.Tests
{
    internal static class NullableTests
    {
        [Fact]
        public static void BasicTests()
        {
            int? n = default(int?);
            
            Assert.Equal(false, n.HasValue);
            Assert.Equal(true, n is null);
            Assert.Equal(true, 7 != n);
            Assert.Equal(0, n.GetHashCode());
            Assert.Equal("", n.ToString());
            Assert.Equal(default(int), n.GetValueOrDefault());
            Assert.Equal(999, n.GetValueOrDefault(999));

            n = new int?(42);
            // TODO: This needs RuntimeExports.Box to work with Nullable<T> properly
            //Assert.Equal(true, n.HasValue);
            Assert.Equal(42, n.Value);
            Assert.Equal(42, (int)n);

            Assert.Equal(true, n is not null);
            Assert.Equal(true, 7 != n);

            // TODO: This needs RuntimeExports.Box to work with Nullable<T> properly
            //Assert.Equal(42, n);
            Assert.Equal(true, n.Equals(42));

            Assert.Equal(true, 42.GetHashCode() == n.GetHashCode());
            Assert.Equal(42.ToString(), n.ToString());
            Assert.Equal(n.GetValueOrDefault(), 42);
            Assert.Equal(n.GetValueOrDefault(999), 42);

            n = 88;
            Assert.Equal(true, n.HasValue);
            Assert.Equal(true, n.Equals(88));
        }
    }
}
