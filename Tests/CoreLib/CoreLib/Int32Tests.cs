namespace CoreLib
{
    public class Int32Tests
    {
        public static bool Parse_Valid()
        {
            var result = true;

            result = result && AssertEqual(0, int.Parse("0"));
            result = result && AssertEqual(0, int.Parse("0000000000000000000000000000000000000000000000000000000000"));
            result = result && AssertEqual(1, int.Parse("0000000000000000000000000000000000000000000000000000000001"));
            result = result && AssertEqual(2147483647, int.Parse("2147483647"));
            result = result && AssertEqual(2147483647, int.Parse("02147483647"));
            result = result && AssertEqual(2147483647, int.Parse("00000000000000000000000000000000000000000000000002147483647"));
            result = result && AssertEqual(123, int.Parse("123\0\0"));

            result = result && AssertEqual(123, int.Parse("123"));

            return result;
        }

        private static bool AssertEqual(int expected, int actual) => expected == actual;
    }
}
