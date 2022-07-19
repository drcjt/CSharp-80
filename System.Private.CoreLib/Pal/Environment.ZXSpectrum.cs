using System.Runtime.InteropServices;


namespace System
{
    public static class Environment
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

        //[DllImport(Libraries.Runtime, EntryPoint = "GETDATETIME")]
        public static DateTime GetDateTime() { return new DateTime(); }

        public static char NewLine => '\r';
    }
}