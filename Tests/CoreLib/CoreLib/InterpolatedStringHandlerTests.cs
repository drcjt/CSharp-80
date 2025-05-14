using System.Runtime.CompilerServices;
using Xunit;

namespace CoreLib
{
    public static class InterpolatedStringHandlerTests
    {
        public static void SingleInterpolation()
        {
            var one = "one";
            var result = $"{one}";
            Assert.Equal("one", result);
        }

        public static void DoubleInterpolation()
        {
            var one = "one";
            var two = "two";
            var result = $"{one}{two}";
            Assert.Equal("onetwo", result);
        }

        public static void TripleInterpolation()
        {
            var one = "one";
            var two = "two";
            var three = "three";
            var result = $"{one}{two}{three}";
            Assert.Equal("onetwothree", result);
        }

        public static void QuadInterpolation()
        {
            var one = "one";
            var two = "two";
            var three = "three";
            var four = "four";

            var result = $"{one}{two}{three}{four}";
            Assert.Equal("onetwothreefour", result);
        }
        
        public static void ToStringAndClear_Clears()
        {
            var handler = new DefaultInterpolatedStringHandler(0, 0);
            handler.AppendLiteral("hi");
            Assert.True(handler.ToStringAndClear().Equals("hi"));
            Assert.True(handler.ToStringAndClear().Equals(string.Empty));
        }

        public static void AppendLiteral()
        {
            var expected = "";
            var actual = new DefaultInterpolatedStringHandler(0, 0);

            foreach (var s in new[] { "", "a", "bc", "def", "this is a long string", "!"})
            {
                expected += s;
                actual.AppendLiteral(s);
            }

            Assert.True(expected.Equals(actual.ToStringAndClear()));
        }

        public static void AppendFormatted()
        {
            var expected = "";
            var actual = new DefaultInterpolatedStringHandler(0, 0);

            foreach (var i in new[] { 1, 25, 43, -72, 999 })
            {
                expected += i.ToString();
                expected += ",";
                actual.AppendFormatted(i);
                actual.AppendLiteral(",");
            }

            Assert.True(expected.Equals(actual.ToStringAndClear()));
        }
    }
}
