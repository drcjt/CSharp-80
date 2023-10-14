namespace System
{
    public readonly partial struct DateTime
    {
        public int Day { get; }
        public int Hour { get; }
        public int Minute { get; }
        public int Second { get; }

        public int TotalSeconds => Second + (Minute * 60) + (Hour * 60 * 60);

        public static DateTime Now { get {  return GetDateTime(); } }

        public DateTime(int day, int hour, int minute, int second)
        {
            Day = day;
            Hour = hour;
            Minute = minute;
            Second = second;
        }
    }
}
