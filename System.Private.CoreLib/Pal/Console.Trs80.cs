using Internal.Runtime;
using System.Runtime.InteropServices;

namespace System
{
    public static partial class Console
    {
        [DllImport(Libraries.Runtime, EntryPoint = "READLINE")]
        private static unsafe extern string InternalReadLine(EEType* stringEEType);

        public static unsafe string ReadLine()
        {
            return InternalReadLine(EETypePtr.EETypePtrOf<string>());
        }

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

        public static bool KeyAvailable => KeyAvail();
        public static unsafe ConsoleKeyInfo ReadKey() => ReadKey(false);
        public static unsafe ConsoleKeyInfo ReadKey(bool intercept)
        {
            char c = (char)KbdScan();

            ConsoleKey k = default;
            if (c == 'w')
                k = ConsoleKey.UpArrow;
            else if (c == 'd')
                k = ConsoleKey.DownArrow;
            else if (c == 's')
                k = ConsoleKey.LeftArrow;
            else if (c == 'a')
                k = ConsoleKey.RightArrow;

            // display key if required
            if (!intercept && c != 0)
            {
                Write(c);
            }

            return new ConsoleKeyInfo(c, k, false, false, false);
        }

        // Fast key test routine
        [DllImport(Libraries.Runtime, EntryPoint = "KeyAvail")]
        private static unsafe extern bool KeyAvail();

        [DllImport(Libraries.Runtime, EntryPoint = "KbdScan")]
        private static unsafe extern int KbdScan();

        public static int WindowHeight { get { return 16; } }
        public static int WindowWidth { get { return 64; } }

        public static void Beep()
        {
            Beep(800, 200);
        }

        [DllImport(Libraries.Runtime, EntryPoint = "Beep")]
        private static extern void InternalBeep(int frequency, int duration);

        public static void Beep(int frequency, int duration)
        {
            // Work out the frequency count for the assembly code routine
            // TODO: Really want to do this calculation as follows
            // frequencyCount = ((1000000 / frequency) - 44.53) / 25.9
            // But we don't have floating point numbers yet

            var frequencyCount = 0;
            for (int count = 122; count > 0; count--)
            {
                var estimatedFreq = 1000000 / (26 * count + 44);

                if (frequency < estimatedFreq)
                {
                    frequencyCount = count;
                    break;
                }
            }

            InternalBeep(frequencyCount, duration * frequency / 1000);
        }
    }
}
