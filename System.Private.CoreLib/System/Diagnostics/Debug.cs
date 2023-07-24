namespace System.Diagnostics
{
    internal static class Debug
    {
        [System.Diagnostics.Conditional("DEBUG")]
        internal static void Assert(bool condition, string message)
        {
            if (!condition) 
            {
                Console.WriteLine(message);
                Environment.Exit(-1);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        internal static void Assert(bool condition)
        {
            if (!condition)
            {
                Environment.Exit(-1);
            }
        }
    }
}
