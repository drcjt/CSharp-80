using System.Runtime.InteropServices;

namespace System
{
    public static partial class Console
    {
        public static unsafe void SetConsoleCursorPosition(sbyte x, sbyte y) { }
        public static unsafe void SetCursorPosition(int x, int y) { }

        private static unsafe bool KeyAvail() { return false; }

        public static unsafe ConsoleKeyInfo ReadKey(bool intercept) { return new ConsoleKeyInfo(); }

        public static bool KeyAvailable => false;
    }
}
