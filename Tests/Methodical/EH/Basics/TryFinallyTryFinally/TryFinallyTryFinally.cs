namespace TryFinallyTryFinally
{
    internal static class Test
    {
        public static int Main()
        {
            try
            {
                Log.Mark(1);
            }
            finally
            {
                Log.Mark(2);
            }

            try
            {
                Log.Mark(3);
            }
            finally
            {
                Log.Mark(4);
            }

            return AssertSequence(new[] { 1, 2, 3, 4 });
        }

        public static void InTry()
        {
            Log.Mark(1); // In try
        }

        public static void InFinally()
        {
            Log.Mark(2); // In catch
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
