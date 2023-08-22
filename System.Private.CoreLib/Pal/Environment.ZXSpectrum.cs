using System.Runtime.InteropServices;


namespace System
{
    public static partial class Environment
    {
        /// <summary>
        /// The ZX Spectrum contains a 3 byte frame counter in 23672-23674 which is incremented every 20ms.
        /// TODO: Write assembly code routine to use this to implement GetTickCount
        /// </summary>
        /// <returns></returns>
        [DllImport(Libraries.Runtime, EntryPoint = "TICKS")]
        private static extern int GetFrameCount();

        /// <summary>
        /// Approximation to TickCount. Note that 20ms frame count is only 24 bit and will
        /// cycle back to 0 roughly every 4 days.
        /// </summary>
        public static int TickCount => GetFrameCount() * 20;

        //DllImport(Libraries.Runtime, EntryPoint = "GETDATETIME")]
        public static DateTime GetDateTime()
        {
            var framesCounter = GetFramesCounter();
            var seconds = (framesCounter / 50) % 60;
            var minutes = (framesCounter / 3000) % 60;
            var hours = (framesCounter / 180000) % 60;
            var days = (framesCounter / 4320000) % 24;
            return new DateTime(days, hours, minutes, seconds);
        }

        private static unsafe int GetFramesCounter()
        {
            return 65536 * *((byte*)23674) + 256 * *((byte*)23673) + *((byte*)23672);
        }

        public static string NewLine => "\r";
    }
}