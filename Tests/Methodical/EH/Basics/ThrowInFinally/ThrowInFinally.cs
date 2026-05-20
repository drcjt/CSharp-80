using System;

namespace ThrowInFinally
{
    internal static class Test
    {
        public static int Main()
        {
            try
            {
                Log.Mark(1); // Main: In Try
                MiddleMethod();
            }
            catch
            {
                Log.Mark(4); // Main: Caught the exception
            }

            return AssertSequence(new[] { 1, 2, 3, 4 });
        }

        public static void MiddleMethod()
        {
            try
            {
                Log.Mark(2); // In try
            }
            finally
            {
                Log.Mark(3); // In finally
                throw new ArgumentException();
            }
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
