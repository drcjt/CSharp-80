namespace LocalGotoInAHandler
{
    internal static class Test
    {
        public static int Main()
        {
            try
            {
                Log.Mark(1);    // in main try
            }
            finally
            {
                Log.Mark(2);    // in main finally

                try
                {
                    Log.Mark(3);    // in inner try 1

                    try
                    {
                        Log.Mark(4);    // in inner try 2

                        goto LABEL;
                    }
                    catch
                    {
                        Log.Mark(5);    // will never see this catch
                    }

                    Log.Mark(6);    // will never see this code, jumping over it

                LABEL:

                    Log.Mark(7);    // Back in inner try 1
                }
                finally
                {
                    Log.Mark(8);    // in inner finally
                }
            }

            return AssertSequence(new[] { 1, 2, 3, 4, 7, 8 });
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
