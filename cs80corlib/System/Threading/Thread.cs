namespace System.Threading
{
    public class Thread
    {
        public static void Sleep(int delayMs)
        {
            // TODO: Convert this to a more accurate runtime routine

            // For TRS-80 Model 1, ROM Routine @ 0x60h is a delay routine
            // pass in counter in BC, each loop is 14.65 microseconds long

            // 14.64 microseconds = 0.01465 milliseconds

            // so approx 68.25 counts per millisecond

            // So delayMs * 68 = count

            // But count is 16 bit so max count = 65535, so max delay = 963 milliseconds

            // Alternative idea is to use ROM routine to delay for 1 millisecond e.g. always use a count of 68
            // and then just call the asm routine delayMs times

            int start = Environment.TickCount;
            while (Environment.TickCount - start < delayMs)
            {
                // Do nothing
            }
        }
    }
}
