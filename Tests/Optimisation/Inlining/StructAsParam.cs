using System.Runtime.CompilerServices;
using Xunit;

namespace Inlining
{
    internal struct TheStruct
    {
        public string fieldinStruct;
    }

    public static class StructAsParam
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void StructTaker_Inline(TheStruct s)
        {
            s.fieldinStruct = "xyz";
        }

        public static int Main()
        {
            TheStruct testStruct = new TheStruct();
            testStruct.fieldinStruct = "change_xyz";

            StructTaker_Inline(testStruct);

            Assert.Equal("change_xyz", testStruct.fieldinStruct);

            return 0;
        }
    }
}