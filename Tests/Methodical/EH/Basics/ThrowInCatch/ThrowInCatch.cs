using System;

namespace ThrowInCatch
{
    internal static class Test
    {
        public static int Main()
        {
            try
            {
                try
                {
                    Log.Mark(1); // In try
                    try
                    {
                        Log.Mark(2); // In try2, 1st throw
                        throw new Exception();
                    }
                    catch
                    {
                        Log.Mark(3); // In 1st catch, 2nd throw
                        throw new Exception();
                    }
                }
                catch
                {
                    Log.Mark(4); // In 2nd catch
                }
            }
            catch
            {
                Log.Mark(5); // Unreached
            }
            Log.Mark(6); // Done

            return AssertSequence(new[] { 1, 2, 3, 4, 6 });
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
