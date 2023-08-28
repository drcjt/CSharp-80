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

        // Although screen has 24 lines bottom 2 are reserved for input only
        public static int WindowHeight { get { return 22; } }
        public static int WindowWidth { get { return 32; } }

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
            const int CPUFrequencyInHertz = 3500000;
            var tonePeriod = ((CPUFrequencyInHertz / frequencyInHertz) - 236) / 8;
            var toneDurationInCycles = durationInMilliseconds * frequencyInHertz / 1000;

            InternalBeep(tonePeriod, toneDurationInCycles);
        }
    }
}
