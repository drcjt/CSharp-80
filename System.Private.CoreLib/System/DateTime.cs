namespace System
{
    public readonly partial struct DateTime
        : IComparable,
          IEquatable<DateTime>,
          IComparable<DateTime>
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


        public int CompareTo(object? obj)
        {
            if (obj == null) return 1;

            if (obj is DateTime dt)
            {
                return Compare(this, dt);
            }

            throw new ArgumentException();
        }

        public int CompareTo(DateTime value)
        {
            return Compare(this, value);
        }

        public static int Compare(DateTime t1, DateTime t2)
        {
            int totalSeconds1 = t1.TotalSeconds;
            int totalSeconds2 = t2.TotalSeconds;
            if (totalSeconds1 > totalSeconds2) return 1;
            if (totalSeconds1 < totalSeconds2) return -1;
            return 0;
        }

        public static bool operator <(DateTime t1, DateTime t2) => t1.TotalSeconds < t2.TotalSeconds;
        public static bool operator <=(DateTime t1, DateTime t2) => t1.TotalSeconds <= t2.TotalSeconds;
        public static bool operator >(DateTime t1, DateTime t2) => t1.TotalSeconds > t2.TotalSeconds;
        public static bool operator >=(DateTime t1, DateTime t2) => t1.TotalSeconds >= t2.TotalSeconds;

    }
}
