using System;
using Xunit;

namespace ThrowInFinallyErrPathFn
{
    internal static class Test
    {
        [ActiveIssue("Exception handling doesn't call Finally handlers when exception thrown")]
        public static int Main()
        {
            try
            {
                MiddleMethod();
            }
            catch
            {
                Log.Mark(5); // Pass
            }

            return AssertSequence(new[] { 1, 3, 5 });
        }

        public static void MiddleMethod()
        {
            try
            {

                Log.Mark(1); // In try, throwing
                throw new Exception();
                Log.Mark(2); // Unreached
            }
            finally
            {
                Log.Mark(3); // In finally, throwing
                throw new Exception();
            }
            Log.Mark(4); // Unreached...
        }

        public static int AssertSequence(int[] expected)
        {
            if (Log.Index != expected.Length)
                return 1;

            for (int i = 0; i < expected.Length; i++)
            {
                if (Log.Events[i] != expected[i])
                    return 1;
            }

            return 0;
        }
    }

    public static class Log
    {
        public static int[] Events { get; } = new int[10];
        public static int Index { get; set; }

        public static void Mark(int id) => Events[Index++] = id;
    }
}
