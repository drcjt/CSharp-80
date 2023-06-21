using Internal.Runtime;
using System.Runtime.InteropServices;

namespace System
{
    public static partial class Console
    {
        [DllImport(Libraries.Runtime, EntryPoint = "READLINE")]
        private static unsafe extern String InternalReadLine(EEType* stringEEType);

        public static unsafe string ReadLine()
        {
            return InternalReadLine(EETypePtr.EETypePtrOf<String>());
        }

        [DllImport(Libraries.Runtime, EntryPoint = "SetXY")]
        public static unsafe extern void SetConsoleCursorPosition(sbyte x, sbyte y);

        public static unsafe void SetCursorPosition(int x, int y) 
        {
            SetConsoleCursorPosition((sbyte)x, (sbyte)y);
        }

        [DllImport(Libraries.Runtime, EntryPoint = "SetXY")]
        private static unsafe extern int GetConsoleCursorPosition();

        /*
         * TODO: Needs ValueTuple for this to work
        public static (int Left, int Top) GetCursorPosition()
        {
            var cursorPos = GetConsoleCursorPosition();

            return (cursorPos >> 8, cursorPos & 255);
        }
        */

        private static unsafe bool KeyAvail() { return false; }

        public static unsafe ConsoleKeyInfo ReadKey(bool intercept) { return new ConsoleKeyInfo(); }

        public static bool KeyAvailable => false;

        public static int WindowHeight { get { return 24; } }
        public static int WindowWidth { get { return 32; } }
    }
}
