using System.Diagnostics;

namespace ILCompiler.TypeSystem.Common
{
    public static class AlignmentHelper
    {
        public static int AlignUp(this int val, int alignment)
        {
            Debug.Assert(val >= 0 && alignment >= 0);

            Debug.Assert(0 == (alignment & (alignment - 1)));
            var result = (val + (alignment - 1)) & ~(alignment - 1);
            Debug.Assert(result >= val);

            return result;
        }
    }
}