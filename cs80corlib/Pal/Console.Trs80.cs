using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
    public static class Console
    {
        [Intrinsic]
        public static void Write(Int32 value) { }

        [Intrinsic]
        public static void Write(uint value) { }

        [Intrinsic]
        public static void Write(String str) { }

        [Intrinsic]
        public static void Write(char c) { }

        [DllImport(Libraries.Runtime, EntryPoint = "SetXY")]
        public static unsafe extern void SetConsoleCursorPosition(sbyte x, sbyte y);

        public static unsafe void SetCursorPosition(int x, int y)
        {
            // TODO: See if this can be made more efficient
            /*
			byte* cursorLocation = (byte*)0x4020;
			cursorLocation[0] = (byte)x;
			cursorLocation[1] = (byte)y;
			*/
            SetConsoleCursorPosition((sbyte)x, (sbyte)y);
        }

		[DllImport(Libraries.Runtime, EntryPoint = "WRITE")]
		public static unsafe extern void WriteConsole(Int32 ch);

        [DllImport(Libraries.Runtime, EntryPoint = "CLS")]
        public static unsafe extern void Clear();

        // TODO: This doesn't seem to work properly
        public static bool KeyAvailable => (KbdScan() != 0);

        public static unsafe ConsoleKeyInfo ReadKey()
        {
            return ReadKey(false);
        }
        public static unsafe ConsoleKeyInfo ReadKey(bool intercept)
        {
            char c = (char)KbdScan();

            // Interpret WASD as arrow keys
            ConsoleKey k = default;
            if (c == 'w')
                k = ConsoleKey.UpArrow;
            else if (c == 'd')
                k = ConsoleKey.DownArrow;
            else if (c == 's')
                k = ConsoleKey.LeftArrow;
            else if (c == 'a')
                k = ConsoleKey.RightArrow;

            if (c != 0 && !intercept)
            {
                Write(c);
            }

            return new ConsoleKeyInfo(c, k, false, false, false);
        }

        [DllImport(Libraries.Runtime, EntryPoint = "KbdScan")]
        public static unsafe extern int KbdScan();

        public static void WriteLine()
        {
            Write(Environment.NewLine);
        }

        public static void WriteLine(Int32 value)
        {
            Write(value);
            WriteLine();
        }

        public static void WriteLine(UInt32 value)
        {
            Write(value);
            WriteLine();
        }

        public static void WriteLine(string str)
        {
            // TODO: Really want to use string concatenation here but not sure that will work yet
            Write(str);
            WriteLine();
        }
    }
}
