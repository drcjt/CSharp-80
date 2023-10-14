namespace System
{
    public partial struct DateTime
    {
        private static unsafe int GetFramesCounter()
        {
            return 65536 * *((byte*)23674) + 256 * *((byte*)23673) + *((byte*)23672);
        }

        private static DateTime GetDateTime()
        {
            var framesCounter = GetFramesCounter();
            var seconds = (framesCounter / 50) % 60;
            var minutes = (framesCounter / 3000) % 60;
            var hours = (framesCounter / 180000) % 60;
            var days = (framesCounter / 4320000) % 24;
            return new DateTime(days, hours, minutes, seconds);
        }
    }
}
