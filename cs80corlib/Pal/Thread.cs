namespace System.Threading
{
    public static class Thread
    {
        public static void Sleep(int delayMs)
        {
            // TODO: Convert this to a more accurate runtime routine
            int start = Environment.TickCount;
            while (Environment.TickCount - start < delayMs)
            {
                // Do nothing
            }
        }
    }
}
