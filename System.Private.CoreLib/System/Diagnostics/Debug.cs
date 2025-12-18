using System.Runtime.CompilerServices;

namespace System.Diagnostics
{
    public static class Debug
    {
        [System.Diagnostics.Conditional("DEBUG")]
        public static void Assert(bool condition, string message)
        {
            if (!condition) 
            {
                Console.WriteLine(message);
                Environment.Exit(-1);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Assert(bool condition)
        {
            if (!condition)
            {
                Environment.Exit(-1);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        [Intrinsic]
        internal static void DebugBreak()
        {
            throw new PlatformNotSupportedException();
        }
    }
}
