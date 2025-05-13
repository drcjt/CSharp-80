using Xunit;

namespace System.Tests
{
    internal static class DateTimeTests
    {
        private static readonly DateTime _testDateTime = new DateTime(10, 9, 8, 7);
        public static void Ctor_Int_Int_Int_Int()
        {
            VerifyDateTime(_testDateTime, 10, 9, 8, 7);
        }

        private static void VerifyDateTime(DateTime dateTime, int day, int hour, int minute, int second)
        {
            Assert.Equal(day, dateTime.Day);
            Assert.Equal(hour, dateTime.Hour);
            Assert.Equal(minute, dateTime.Minute);
            Assert.Equal(second, dateTime.Second);
        }

        public static void Equals_Tests()
        {
            EqualsTest(_testDateTime, _testDateTime, true);
            EqualsTest(_testDateTime, new DateTime(11, 10, 9, 8), false);
            EqualsTest(_testDateTime, new DateTime(9, 8, 7, 6), false);
            EqualsTest(_testDateTime, new object(), false);
            EqualsTest(_testDateTime, null, false);
        }

        public static void EqualsTest(DateTime date, object? other, bool expected)
        {
            if (other is DateTime otherDate)
            {
                Assert.Equal(expected, date.Equals(otherDate));
                Assert.Equal(expected, date.GetHashCode().Equals(otherDate.GetHashCode()));

                Assert.Equal(expected, date == otherDate);
                Assert.Equal(!expected, date != otherDate);
            }

            Assert.Equal(expected, date.Equals(other));
        }
    }
}
