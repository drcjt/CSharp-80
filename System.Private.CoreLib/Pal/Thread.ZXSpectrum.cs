using System.Runtime.InteropServices;

namespace System.Threading
{
    public class Thread
    {
        // This delays by approx 20 milliseconds for each delay unit
        [DllImport(Libraries.Runtime, EntryPoint = "Delay")]
        private static unsafe extern void Delay(int delay);

        public static void Sleep(int delayMs)
        {
            var delay = delayMs / 20;

            while (delay > 65535)
            {
                Delay(65535);
                delay -= 65535;
            }

            Delay(delay);
        }
    }
}
