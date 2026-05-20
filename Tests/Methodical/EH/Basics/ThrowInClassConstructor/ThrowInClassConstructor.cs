using System;
using Xunit;

namespace ThrowInClassConstructor
{
    public class Foo
    {
        public static int x;

        static Foo()
        {
            int y = 0;
            x = 5 / y;
        }
    }

    internal static class Test
    {
        [ActiveIssue("Exception handling doesn't call Finally handlers when exception thrown")]
        public static int Main()
        {
            int i = 0;
            try
            {
                Log.Mark(1); // try
                try
                {
                    Log.Mark(2); // try
                    i = Foo.x;
                }
                finally
                {
                    Log.Mark(3); // finally;
                    i = Foo.x;
                }
            }
            catch (Exception)
            {
                Log.Mark(4); // catch
            }

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
        public static int[] Events { get; } = new int[10];
        public static int Index { get; set; }

        public static void Mark(int id) => Events[Index++] = id;
    }
}
