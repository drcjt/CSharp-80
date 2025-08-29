using Xunit;

namespace System.Tests
{
    internal static class DateTimeTests
    {
        private static readonly DateTime _testDateTime9 = new(9, 8, 7, 6);
        private static readonly DateTime _testDateTime10 = new(10, 9, 8, 7);
        private static readonly DateTime _testDateTime11 = new(11, 10, 9, 8);

        [Fact]
        public static void Ctor_Int_Int_Int_Int()
        {
            VerifyDateTime(_testDateTime10, 10, 9, 8, 7);
        }

        private static void VerifyDateTime(DateTime dateTime, int day, int hour, int minute, int second)
        {
            Assert.Equal(day, dateTime.Day);
            Assert.Equal(hour, dateTime.Hour);
            Assert.Equal(minute, dateTime.Minute);
            Assert.Equal(second, dateTime.Second);
        }

        [Fact]
        public static void Equals_Tests()
        {
            EqualsTest(_testDateTime10, _testDateTime10, true);
            EqualsTest(_testDateTime10, _testDateTime11, false);
            EqualsTest(_testDateTime10, _testDateTime9, false);
            EqualsTest(_testDateTime10, new object(), false);
            EqualsTest(_testDateTime10, null, false);
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

        [Fact]
        public static void CompareTo_Invoke_ReturnsExpected_Tests()
        {
            CompareTo_Invoke_ReturnsExpected(_testDateTime10, _testDateTime10, 0);
            CompareTo_Invoke_ReturnsExpected(_testDateTime10, _testDateTime11, -1);
            CompareTo_Invoke_ReturnsExpected(_testDateTime10, _testDateTime9, 1);
            CompareTo_Invoke_ReturnsExpected(_testDateTime10, null, 1);
        }

        public static void CompareTo_Invoke_ReturnsExpected(DateTime date, object? other, int expected)
        {
            if (other is DateTime otherDate)
            {
                Assert.Equal(expected, date.CompareTo(otherDate));
                Assert.Equal(expected, DateTime.Compare(date, otherDate));

                Assert.Equal(expected > 0, date > otherDate);
                Assert.Equal(expected >= 0, date >= otherDate);
                Assert.Equal(expected < 0, date < otherDate);
                Assert.Equal(expected <= 0, date <= otherDate);
            }

            Assert.Equal(expected, date.CompareTo(other));
        }

        [Fact]
        public static void CompareTo_NotDateTime_ThrowsArgumentException()
        {
            try
            {
                DateTime.Now.CompareTo(new object());
            }
            catch (ArgumentException)
            {
                return;
            }

            Assert.Fail("");
        }
    }
}
