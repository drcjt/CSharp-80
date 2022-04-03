using System.Runtime.InteropServices;

namespace System
{
    public static class Environment
    {
        /// <summary>
        /// The TRS80 Model 1 25ms heartbeat seems to go up to only approx 15200 before cycling back to 0
        /// </summary>
        /// <returns></returns>
        [DllImport(Libraries.Runtime, EntryPoint = "TICKS")]
        private static extern int GetTickCount();

        /// <summary>
        /// Approximation to TickCount. Note that 25ms heartbeat count is only 16 bit and seems to 
        /// cycle back to 0 when it gets to approx 15200. This gives a range for TickCount from 0
        /// to 380000 ms or 6 minutes.
        /// </summary>
        public static int TickCount => GetTickCount() * 25;

        [DllImport(Libraries.Runtime, EntryPoint = "GETDATETIME")]
        public static extern DateTime GetDateTime();

        public static char NewLine => '\n';
    }
}
