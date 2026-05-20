using System;
using Xunit;

namespace TryFinallyTryCatch
{
    internal static class Test
    {
        [ActiveIssue("Exception handling doesn't call Finally handlers when exception thrown")]
        public static int Main()
        {
            try
            {
                InTry1();
                try
                {
                    InTry2();
                    throw new Exception();
                }
                finally
                {
                    InFinally();
                }
            }
            catch (Exception)
            {
                Log.Mark(5); // Caught an exception
                InCatch();
            }

            return AssertSequence(new[] { 1, 2, 3, 5, 4 });
        }

        public static void InTry1()
        {
            Log.Mark(1); // in Try catch
        }

        public static void InTry2()
        {
            Log.Mark(2); // in Try finally
        }

        public static void InFinally()
        {
            Log.Mark(3); // in Finally
        }

        public static void InCatch()
        {
            Log.Mark(4); // in Catch
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
        public static int[] Events = new int[5];
        public static int Index;

        public static void Mark(int id) => Events[Index++] = id;
    }
}
