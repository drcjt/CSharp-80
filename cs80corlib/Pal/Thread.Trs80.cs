using System.Runtime.InteropServices;

namespace System.Threading
{
    public class Thread
    {
        // This delays by approx 14.65 microseconds for each delay unit
        [DllImport(Libraries.Runtime, EntryPoint = "Delay")]
        public static unsafe extern void Delay(int delay);

        public static void Sleep(int delayMs)
        {
            while (delayMs > 960)
            {
                Delay(65535);
                delayMs -= 960;
            }

            Delay(delayMs * 68);
        }
    }
}
