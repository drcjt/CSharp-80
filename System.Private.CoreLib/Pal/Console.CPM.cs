using Internal.Runtime;
using System.Runtime.InteropServices;

namespace System
{
    public static partial class Console
    {
        public static string ReadLine()
        {
            return null;
        }

        public static void SetCursorPosition(int x, int y)
        {
        }

        public static bool KeyAvailable => false;
        public static ConsoleKeyInfo ReadKey() => ReadKey(false);
        public static ConsoleKeyInfo ReadKey(bool intercept)
        {
            return new ConsoleKeyInfo('x', 0, false, false, false);
        }

        public static int WindowHeight { get { return 24; } }
        public static int WindowWidth { get { return 80; } }

        public static void Beep(int frequencyInHertz, int durationInMilliseconds)
        {
            // No sounds support on CPM yet
        }
    }
}
