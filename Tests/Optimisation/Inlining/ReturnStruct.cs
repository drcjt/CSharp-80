using System.Runtime.CompilerServices;
using Xunit;

namespace Inlining
{
    public struct MyStruct
    {
        public string str;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MyStruct MakeStruct(string s)
        {
            MyStruct ms;
            ms.str = s;
            return ms;
        }
    }

    public static class Inline
    {
        public static int Main()
        {
            MyStruct ms = MyStruct.MakeStruct("Hello, World!");

            Assert.Equal("Hello, World!", ms.str);

            return 0;
        }
    }
}