using System;

namespace TryThrowCatch
{
    internal static class Test
    {
        public static int Main()
        {
            try
            {
                Log.Mark(1); // In try
                throw new Exception();
            }
            catch (Exception e)
            {
                Log.Mark(2); // In catch
            }
            finally
            {
                Log.Mark(3); // In finally
            }
            Log.Mark(4); // Done

            return AssertSequence(new[] { 1, 2, 3, 4 });
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
