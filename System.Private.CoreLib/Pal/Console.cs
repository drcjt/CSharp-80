using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
    public enum ConsoleKey
    {
        None = 0x0,
        Backspace = 0x8,
        Tab = 0x9,
        Clear = 0x0C,
        Enter = 0x0D,
        Escape = 0x1B,
        Spacebar = 0x20,
        LeftArrow = 0x25,
        UpArrow = 0x26,
        RightArrow = 0x27,
        DownArrow = 0x28,
        D0 = 0x30,
        D1 = 0x31,
        D2 = 0x32,
        D3 = 0x33,
        D4 = 0x34,
        D5 = 0x35,
        D6 = 0x36,
        D7 = 0x37,
        D8 = 0x38,
        D9 = 0x39,
        A = 0x41,
        B = 0x42,
        C = 0x43,
        D = 0x44,
        E = 0x45,
        F = 0x46,
        G = 0x47,
        H = 0x48,
        I = 0x49,
        J = 0x4A,
        K = 0x4B,
        L = 0x4C,
        M = 0x4D,
        N = 0x4E,
        O = 0x4F,
        P = 0x50,
        Q = 0x51,
        R = 0x52,
        S = 0x53,
        T = 0x54,
        U = 0x55,
        V = 0x56,
        W = 0x57,
        X = 0x58,
        Y = 0x59,
        Z = 0x5A,
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
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public static extern void Write(Int32 value);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public static extern void Write(uint value);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public static extern void Write(string str);

        [Intrinsic]
        public static void Write(char c) 
        {
            // This is still intrinsic as it allows for the native
            // code to be inlined
            throw new PlatformNotSupportedException();
        }

        [DllImport(Libraries.Runtime, EntryPoint = "CLS")]
        public static unsafe extern void Clear();

        public static void WriteLine()
        {
            Write(Environment.NewLine);
        }

        public static void WriteLine(char value)
        {
            Write(value);
            WriteLine();
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
            Write(str);
            WriteLine();
        }
    }
}
