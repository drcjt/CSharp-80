using Internal.Runtime;
using System.Runtime.InteropServices;

namespace System
{
    public static partial class Console
    {
        [DllImport(Libraries.Runtime, EntryPoint = "READLINE")]
        private static unsafe extern string InternalReadLine(EETypePtr stringEEType);

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
            var c = (char)KbdScan();

            ConsoleKey k = (ConsoleKey)c;

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


        [DllImport(Libraries.Runtime, EntryPoint = "Beep")]
        private static extern void InternalBeep(int tonePeriod, int toneDurationInCycles);

        public static void Beep()
        {
            const int BeepFrequencyInHz = 800;
            const int BeepDurationInMs = 200;
            Beep(BeepFrequencyInHz, BeepDurationInMs);
        }

        public static void Beep(int frequencyInHertz, int durationInMilliseconds)
        {
            const int CPUFrequencyInHertz = 1000000;
            var tonePeriod = ((CPUFrequencyInHertz / frequencyInHertz) - 44) / 26;
            var toneDurationInCycles = durationInMilliseconds * frequencyInHertz / 1000;

            InternalBeep(tonePeriod, toneDurationInCycles);
        }
    }
}
