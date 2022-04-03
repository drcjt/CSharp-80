namespace System
{
    public readonly struct DateTime
    {
        public readonly int Day;
        public readonly int Hour;
        public readonly int Minute;
        public readonly int Second;

        public int GetTotalSeconds()
        { 
            // TODO: This isn't working, note properties aren't either on structs
            return Second + (Minute * 60) + (Hour * 60 * 60);
        }

        public DateTime(int day, int hour, int minute, int second)
        {
            Day = day;
            Hour = hour;
            Minute = minute;
            Second = second;
        }
    }
}
