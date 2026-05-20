using System;

namespace MultiHandler
{
    internal static class Test
    {
        public static int Main()
        {
            try
            {
                Log.Mark(1); // In try
                throw new ArithmeticException();
            }
            catch (DivideByZeroException)
            {
                Log.Mark(2); // Caught DivideByZeroException
            }
            catch (ArithmeticException)
            {
                Log.Mark(3); // Caught ArithmeticException
            }
            catch (Exception)
            {
                Log.Mark(4); // Caught Exception
            }

            return AssertSequence(new[] { 1, 3 });
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
