using System;

namespace CatchRetToInnerTry
{
    internal static class Test
    {
        public static int Main()
        {
            int i = 3;

            beginloop:

            try
            {
                try
                {
                    if (i == 3)
                    {
                        throw new IndexOutOfRangeException();
                    }
                    else if (i == 4)
                    {
                        Log.Mark(2);    // bye
                    }
                }
                catch (Exception e)
                {
                    Log.Mark(1);    // Caught an exception
                    i++;
                    goto beginloop;
                }
            }
            finally
            {
                Log.Mark(3);    // In outer finally
            }

            return AssertSequence(new[] { 1, 3, 2, 3 });
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
