using System.Runtime.CompilerServices;
using Xunit;

namespace Inlining
{
    public static class Program
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Foo_NoInline(string s)
        {
            Assert.Equal("Original string", s);
            s = "New string";
            Assert.Equal("New string", s);
        }

        public static int Main()
        {
            string orig = "Original string";
            Foo_NoInline(orig);
            Assert.Equal("Original string", orig);

            return 0;
        }
    }
}