namespace System
{
    public readonly partial struct DateTime : IEquatable<DateTime>
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

        public override int GetHashCode() => TotalSeconds;
        public override bool Equals(object? obj)
        {
            return obj is DateTime dt && this == dt;
        }

        public bool Equals(DateTime value) => this == value;

        public static bool operator ==(DateTime d1, DateTime d2) => d1.TotalSeconds == d2.TotalSeconds;
        public static bool operator !=(DateTime d1, DateTime d2) => !(d1 == d2);

    }
}
