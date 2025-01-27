namespace System.Tests
{
    internal static class DateTimeTests
    {
        public static void Ctor_Int_Int_Int_Int()
        {
            var datetime = new DateTime(10, 9, 8, 7);
            VerifyDateTime(datetime, 10, 9, 8, 7);
        }

        private static void VerifyDateTime(DateTime dateTime, int day, int hour, int minute, int second)
        {
            Assert.AreEqual(day, dateTime.Day);
            Assert.AreEqual(hour, dateTime.Hour);
            Assert.AreEqual(minute, dateTime.Minute);
            Assert.AreEqual(second, dateTime.Second);
        }

        public static void Equals_Tests()
        {
            EqualsTest(new DateTime(10, 9, 8, 7), new DateTime(10, 9, 8, 7), true);
            EqualsTest(new DateTime(10, 9, 8, 7), new DateTime(11, 10, 9, 8), false);
            EqualsTest(new DateTime(10, 9, 8, 7), new DateTime(9, 8, 7, 6), false);
            EqualsTest(new DateTime(10, 9, 8, 7), new object(), false);
            EqualsTest(new DateTime(10, 9, 8, 7), null, false);
        }

        public static void EqualsTest(DateTime date, object? other, bool expected)
        {
            if (other is DateTime otherDate)
            {
                Assert.AreEqual(expected, date.Equals(otherDate));
                Assert.AreEqual(expected, date.GetHashCode().Equals(otherDate.GetHashCode()));

                Assert.AreEqual(expected, date == otherDate);
                Assert.AreEqual(!expected, date != otherDate);
            }

            Assert.AreEqual(expected, date.Equals(other));

        }
    }
}
