using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
    public enum ConsoleKey
    {
        Escape = 27,
        LeftArrow = 37,
        UpArrow = 38,
        RightArrow = 39,
        DownArrow = 40
    }

    public readonly struct ConsoleKeyInfo
    {
        public ConsoleKeyInfo(char keyChar, ConsoleKey key, bool shift, bool alt, bool control)
        {
            Key = key;
            KeyChar = keyChar;
        }

        public readonly ConsoleKey Key;
        public readonly char KeyChar;
    }

    public static partial class Console
    {
        [Intrinsic]
        public static void Write(Int32 value) { }

        [Intrinsic]
        public static void Write(uint value) { }

        [Intrinsic]
        public static void Write(String str) { }

        [Intrinsic]
        public static void Write(char c) { }

        [DllImport(Libraries.Runtime, EntryPoint = "CLS")]
        public static unsafe extern void Clear();

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
